using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

string FixEncoding(string text) {
    if (string.IsNullOrEmpty(text)) return text;
    try {
        if (text.Contains("Ã") || text.Contains("É") || text.Contains("Ä") || text.Contains("Å") || text.Contains("?") || text.Contains("?")) {
            var bytes = Encoding.GetEncoding(1252).GetBytes(text);
            var utf8 = Encoding.UTF8.GetString(bytes);
            if (utf8 != text && !utf8.Contains("")) return utf8;
        }
    } catch {}
    return text;
}

void ProcessFiles(string dir) {
    var files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
    foreach (var file in files) {
        if (file.Contains("\\bin\\") || file.Contains("\\obj\\") || file.Contains("\\.git\\")) continue;
        if (file.EndsWith(".cs") || file.EndsWith(".cshtml") || file.EndsWith(".json")) {
            var content = File.ReadAllText(file, Encoding.UTF8);
            var fixedContent = FixEncoding(content);
            if (fixedContent != content) {
                File.WriteAllText(file, fixedContent, Encoding.UTF8);
                Console.WriteLine("Fixed file: " + file);
            }
        }
    }
}

ProcessFiles(".");

var dbPath = "ehisseler.db";
if (File.Exists(dbPath)) {
    using var conn = new SqliteConnection($"Data Source={dbPath}");
    conn.Open();
    var tables = new List<string>();
    using (var cmd = conn.CreateCommand()) {
        cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) {
            tables.Add(reader.GetString(0));
        }
    }

    foreach (var table in tables) {
        var cols = new List<string>();
        using (var cmd = conn.CreateCommand()) {
            cmd.CommandText = $"PRAGMA table_info({table});";
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) {
                var type = reader.GetString(2).ToUpper();
                if (type.Contains("TEXT") || type.Contains("VARCHAR") || type == "") {
                    cols.Add(reader.GetString(1));
                }
            }
        }

        if (cols.Count == 0) continue;

        var rowsToUpdate = new List<Dictionary<string, object>>();
        using (var cmd = conn.CreateCommand()) {
            cmd.CommandText = $"SELECT rowid, {string.Join(", ", cols)} FROM {table}";
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) {
                var rowid = reader.GetInt64(0);
                var rowMap = new Dictionary<string, object>();
                rowMap["rowid"] = rowid;
                bool updated = false;
                for (int i = 0; i < cols.Count; i++) {
                    if (!reader.IsDBNull(i + 1)) {
                        var val = reader.GetString(i + 1);
                        var fixedVal = FixEncoding(val);
                        if (fixedVal != val) {
                            updated = true;
                            val = fixedVal;
                        }
                        rowMap[cols[i]] = val;
                    } else {
                        rowMap[cols[i]] = null;
                    }
                }
                if (updated) {
                    rowsToUpdate.Add(rowMap);
                }
            }
        }

        foreach (var row in rowsToUpdate) {
            using var cmd = conn.CreateCommand();
            var sets = new List<string>();
            foreach (var col in cols) {
                sets.Add($"{col} = @{col}");
                cmd.Parameters.AddWithValue($"@{col}", row[col] ?? DBNull.Value);
            }
            cmd.CommandText = $"UPDATE {table} SET {string.Join(", ", sets)} WHERE rowid = @rowid";
            cmd.Parameters.AddWithValue("@rowid", row["rowid"]);
            cmd.ExecuteNonQuery();
        }
    }
    Console.WriteLine("Fixed DB");
}
