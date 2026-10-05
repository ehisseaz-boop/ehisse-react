import sqlite3
import datetime

conn = sqlite3.connect('ehisseler.db')
c = conn.cursor()

c.execute('SELECT Id, CreatedDate FROM SpareParts')
parts = c.fetchall()

count = 0
for part in parts:
    part_id = part[0]
    if count % 2 == 0:
        # Today
        new_date = datetime.datetime.utcnow().strftime('%Y-%m-%d %H:%M:%S')
    else:
        # Past
        past_date = datetime.datetime.utcnow() - datetime.timedelta(days=count+2, hours=count)
        new_date = past_date.strftime('%Y-%m-%d %H:%M:%S')
    
    c.execute('UPDATE SpareParts SET CreatedDate = ? WHERE Id = ?', (new_date, part_id))
    count += 1

conn.commit()
conn.close()
print(f"Updated {count} records.")
