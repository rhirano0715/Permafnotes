# -*- coding: utf8 -*-

import time
import datetime
import pathlib

def main():
    base_dir = pathlib.Path(r'C:\Users\r-h-0\OneDrive\Application\Permafnotes')
    notes_dir = base_dir / "notes"

    notes: list(str) = []
    for file in sorted(notes_dir.iterdir(), key=lambda x: str(x.name)):
        notes.append(file.read_text(encoding="utf8"))

    json_text = f"[{','.join(notes)}]"

    cache_file = base_dir / "cache.json"
    cache_file.write_text(json_text, encoding="utf8")
    print(f"Updated Permafnotes cache. {datetime.datetime.now()}")

if __name__ == "__main__":
    while(True):
        main()
        time.sleep(10)
