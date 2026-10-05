using System;
using System.IO;
using System.Text;

string FixEncoding(string text) {
    try {
        // text is actually a UTF-8 string that was read as Windows-1252 or ISO-8859-1
        var bytes = Encoding.GetEncoding(1252).GetBytes(text);
        return Encoding.UTF8.GetString(bytes);
    } catch {
        return text;
    }
}

string test = ""Telefon ehtiyat hissələri"";
Console.WriteLine(FixEncoding(test));
