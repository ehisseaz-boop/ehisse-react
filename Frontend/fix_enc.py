import os

replacements = {
    'É™': 'ə',
    'Ã§': 'ç',
    'Ä±': 'ı',
    'ÅŸ': 'ş',
    'Ã¶': 'ö',
    'Ã¼': 'ü',
    'Æ ': 'Ə',
    'Æ': 'Ə',
    'Ä°': 'İ',
    'ÄŸ': 'ğ',
    'Åž': 'Ş',
    'Ã–': 'Ö',
    'Ãœ': 'Ü',
    'Ã‡': 'Ç',
    'Å': 'Ş'
}

def fix_file(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    for bad, good in replacements.items():
        content = content.replace(bad, good)
        
    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(content)

files = [
    r'C:\Users\user\Desktop\e-hisseler\Frontend\src\pages\Login.jsx',
    r'C:\Users\user\Desktop\e-hisseler\Frontend\src\pages\Register.jsx',
    r'C:\Users\user\Desktop\e-hisseler\Frontend\src\pages\admin\AdminRequests.jsx'
]

for file in files:
    if os.path.exists(file):
        fix_file(file)
        print(f"Fixed {file}")
    else:
        print(f"Not found {file}")
