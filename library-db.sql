-- ============================================================
-- library-db.sql
-- Schema + mocked seed data for the Library database (SQLite).
-- Generated from the EF Core code-first model.
--
-- Seeded accounts (passwords are PBKDF2/HMAC-SHA256 hashes):
--   admin@library.com    / Admin123!     (Admin)
--   customer@library.com / Customer123!  (Customer)
--
-- GUID primary keys are stored as 16-byte BLOBs (X'...').
-- ============================================================

CREATE TABLE IF NOT EXISTS "Authors" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Authors" PRIMARY KEY,
    "FirstName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "Biography" TEXT NULL,
    "CreatedAtUtc" TEXT NOT NULL
);
INSERT INTO Authors VALUES('A1111111-1111-1111-1111-111111111111','George','Orwell','English novelist and essayist.','2026-07-03 13:02:46.170733');
INSERT INTO Authors VALUES('A2222222-2222-2222-2222-222222222222','Jane','Austen','English novelist known for romantic fiction.','2026-07-03 13:02:46.170775');
INSERT INTO Authors VALUES('A3333333-3333-3333-3333-333333333333','J.R.R.','Tolkien','Author of high-fantasy classics.','2026-07-03 13:02:46.170775');
INSERT INTO Authors VALUES('A4444444-4444-4444-4444-444444444444','Terry','Pratchett','Author of the Discworld series.','2026-07-03 13:02:46.170776');
INSERT INTO Authors VALUES('A5555555-5555-5555-5555-555555555555','Neil','Gaiman','Author of fantasy and graphic novels.','2026-07-03 13:02:46.170776');
CREATE TABLE IF NOT EXISTS "Books" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Books" PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "Price" decimal(18,2) NOT NULL,
    "Discount" decimal(18,2) NOT NULL,
    "PublicationDate" TEXT NULL,
    "ImageUrl" TEXT NULL,
    "CreatedAtUtc" TEXT NOT NULL,
    "UpdatedAtUtc" TEXT NULL
);
INSERT INTO Books VALUES('B1111111-1111-1111-1111-111111111111','1984','A dystopian social science fiction novel about totalitarian control.',19.98999999999999843,2,'1949-06-08 00:00:00','https://covers.openlibrary.org/b/id/7222246-M.jpg','2026-07-03 13:02:46.175014',NULL);
INSERT INTO Books VALUES('B2222222-2222-2222-2222-222222222222','Animal Farm','A satirical allegorical novella about a farm animal rebellion.',14.99000000000000021,0,'1945-08-17 00:00:00','https://covers.openlibrary.org/b/id/8575741-M.jpg','2026-07-03 13:02:46.175098',NULL);
INSERT INTO Books VALUES('B3333333-3333-3333-3333-333333333333','Pride and Prejudice','A romantic novel of manners set in Georgian England.',12.5,1.5,'1813-01-28 00:00:00','https://covers.openlibrary.org/b/id/8091016-M.jpg','2026-07-03 13:02:46.175101',NULL);
INSERT INTO Books VALUES('B4444444-4444-4444-4444-444444444444','The Hobbit','A fantasy adventure following Bilbo Baggins on a quest.',24.98999999999999844,5,'1937-09-21 00:00:00','https://covers.openlibrary.org/b/id/6979861-M.jpg','2026-07-03 13:02:46.175102',NULL);
INSERT INTO Books VALUES('B5555555-5555-5555-5555-555555555555','Good Omens','A comedy about the birth of the Antichrist and the coming apocalypse.',22,3,'1990-05-01 00:00:00','https://covers.openlibrary.org/b/id/8778971-M.jpg','2026-07-03 13:02:46.175102',NULL);
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Users" PRIMARY KEY,
    "Email" TEXT NOT NULL,
    "FullName" TEXT NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    "Role" INTEGER NOT NULL,
    "CreatedAtUtc" TEXT NOT NULL
);
INSERT INTO Users VALUES('11111111-1111-1111-1111-111111111111','admin@library.com','Library Administrator','100000.SuOmkbdzwflGXtQZklFV4w==.8mxbR9tAkSTOpiwJoVnnlQ0Zh8RRPAaeKm0D/u5uC3c=',1,'2026-07-03 13:02:46.125349');
INSERT INTO Users VALUES('22222222-2222-2222-2222-222222222222','customer@library.com','Demo Customer','100000.GzIOB+fgLWv/55Hgp7y1LQ==.H7TC5kS6ceh4T78ZCKNMnpK3+6x4hIOBYl4+xZoFXCk=',0,'2026-07-03 13:02:46.140766');
CREATE TABLE IF NOT EXISTS "BookAuthors" (
    "AuthorsId" TEXT NOT NULL,
    "BooksId" TEXT NOT NULL,
    CONSTRAINT "PK_BookAuthors" PRIMARY KEY ("AuthorsId", "BooksId"),
    CONSTRAINT "FK_BookAuthors_Authors_AuthorsId" FOREIGN KEY ("AuthorsId") REFERENCES "Authors" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_BookAuthors_Books_BooksId" FOREIGN KEY ("BooksId") REFERENCES "Books" ("Id") ON DELETE CASCADE
);
INSERT INTO BookAuthors VALUES('A1111111-1111-1111-1111-111111111111','B1111111-1111-1111-1111-111111111111');
INSERT INTO BookAuthors VALUES('A1111111-1111-1111-1111-111111111111','B2222222-2222-2222-2222-222222222222');
INSERT INTO BookAuthors VALUES('A2222222-2222-2222-2222-222222222222','B3333333-3333-3333-3333-333333333333');
INSERT INTO BookAuthors VALUES('A3333333-3333-3333-3333-333333333333','B4444444-4444-4444-4444-444444444444');
INSERT INTO BookAuthors VALUES('A4444444-4444-4444-4444-444444444444','B5555555-5555-5555-5555-555555555555');
INSERT INTO BookAuthors VALUES('A5555555-5555-5555-5555-555555555555','B5555555-5555-5555-5555-555555555555');
CREATE INDEX "IX_Authors_LastName" ON "Authors" ("LastName");
CREATE INDEX "IX_BookAuthors_BooksId" ON "BookAuthors" ("BooksId");
CREATE INDEX "IX_Books_Name" ON "Books" ("Name");
CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");
