SELECT t.*, nt.*, n.*
FROM notes AS n
LEFT JOIN notes_tags AS nt ON n.id = nt.note_id
LFT JOIN tags AS t ON nt.tag_id = t.id
ORDER BY n.id DESC, t.id DESC;
