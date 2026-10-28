CREATE TABLE IF NOT EXISTS "Subscribers"
(
    "Id" uuid PRIMARY KEY,
    "Email" varchar(100) NOT NULL UNIQUE,
    "CreatedAt" timestamp with time zone NOT NULL
);

CREATE INDEX IF NOT EXISTS "IX_Subscribers_CreatedAt"
    ON "Subscribers" ("CreatedAt" DESC);


CREATE TABLE IF NOT EXISTS feature_flags
(
    name VARCHAR(100) PRIMARY KEY,
    enabled BOOLEAN NOT NULL DEFAULT FALSE
);

INSERT INTO feature_flags (name, enabled)
VALUES ('SubscriberService', FALSE)
ON CONFLICT (name) DO NOTHING;