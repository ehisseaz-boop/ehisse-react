import os
import sqlite3

def fix_text(text):
    if not isinstance(text, str):
        return text
    try:
        # Check if text contains typical mojibake characters
        if 'Ã' in text or 'É' in text or 'Ä' in text or 'Å' in text:
            # It's likely utf-8 interpreted as cp1252 or similar
            return text.encode('cp1252').decode('utf-8')
    except:
        pass
    return text

def process_file(filepath):
    try:
        with open(filepath, 'r', encoding='utf-8') as f:
            content = f.read()
            
        fixed_content = fix_text(content)
        
        if fixed_content != content:
            with open(filepath, 'w', encoding='utf-8') as f:
                f.write(fixed_content)
            print(f"Fixed file: {filepath}")
    except Exception as e:
        pass

def process_directory(directory):
    for root, dirs, files in os.walk(directory):
        if 'bin' in dirs: dirs.remove('bin')
        if 'obj' in dirs: dirs.remove('obj')
        if '.git' in dirs: dirs.remove('.git')
        
        for file in files:
            if file.endswith('.cs') or file.endswith('.cshtml') or file.endswith('.json'):
                process_file(os.path.join(root, file))

def fix_db(db_path):
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()
    
    # Get all tables
    cursor.execute("SELECT name FROM sqlite_master WHERE type='table';")
    tables = cursor.fetchall()
    
    for table_name in tables:
        table = table_name[0]
        # Get columns
        cursor.execute(f"PRAGMA table_info({table})")
        columns = cursor.fetchall()
        
        text_cols = [col[1] for col in columns if 'TEXT' in col[2].upper() or 'VARCHAR' in col[2].upper() or col[2] == '']
        
        if not text_cols:
            continue
            
        # Select rows
        cursor.execute(f"SELECT rowid, {', '.join(text_cols)} FROM {table}")
        rows = cursor.fetchall()
        
        for row in rows:
            rowid = row[0]
            updated = False
            new_values = []
            
            for i in range(1, len(row)):
                val = row[i]
                if isinstance(val, str):
                    fixed_val = fix_text(val)
                    if fixed_val != val:
                        updated = True
                        val = fixed_val
                new_values.append(val)
                
            if updated:
                set_clause = ', '.join([f"{col} = ?" for col in text_cols])
                cursor.execute(f"UPDATE {table} SET {set_clause} WHERE rowid = ?", (*new_values, rowid))
                
    conn.commit()
    conn.close()
    print(f"Fixed db: {db_path}")

print("Processing files...")
process_directory('.')
if os.path.exists('ehisseler.db'):
    print("Processing db...")
    fix_db('ehisseler.db')
print("Done.")
