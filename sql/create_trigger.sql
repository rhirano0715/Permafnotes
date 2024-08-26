CREATE TRIGGER trg_update_timestamp
ON notes
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE notes
    SET updated_at = SYSDATETIMEOFFSET()
    FROM inserted i
    WHERE notes.id = i.id; -- idはテーブルのプライマリキーに合わせて変更してください
END;


