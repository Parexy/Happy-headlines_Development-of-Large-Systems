CREATE TABLE IF NOT EXISTS feature_flags
(
    name VARCHAR(100) PRIMARY KEY,
    enabled BOOLEAN NOT NULL DEFAULT FALSE
);

INSERT INTO feature_flags (name, enabled)
VALUES ('SubscriberService', FALSE)
ON CONFLICT (name) DO NOTHING;

CREATE TABLE IF NOT EXISTS subscribers
(
    id UUID PRIMARY KEY,
    email VARCHAR(320) NOT NULL UNIQUE,
    created_at TIMESTAMPTZ NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_subscribers_created_at
    ON subscribers (created_at DESC);
