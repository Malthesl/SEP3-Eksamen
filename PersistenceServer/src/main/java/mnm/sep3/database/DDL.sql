DROP SCHEMA IF EXISTS sep3_eksamen cascade;
CREATE SCHEMA sep3_eksamen;
SET SCHEMA 'sep3_eksamen';

ALTER SCHEMA sep3_eksamen OWNER TO sep3_eksamen;

ALTER DEFAULT PRIVILEGES IN SCHEMA sep3_eksamen
    GRANT ALL PRIVILEGES ON TABLES TO sep3_eksamen;

ALTER DEFAULT PRIVILEGES IN SCHEMA sep3_eksamen
    GRANT ALL PRIVILEGES ON SEQUENCES TO sep3_eksamen;

CREATE DOMAIN visibility AS VARCHAR NOT NULL CHECK (VALUE IN ('public', 'private'));

CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(16) UNIQUE NOT NULL,
    password VARCHAR NOT NULL
);

CREATE TABLE quizzes (
    id SERIAL PRIMARY KEY,
    title VARCHAR NOT NULL,
    visibility visibility,
    creator_id INT NOT NULL REFERENCES users (id)
);

CREATE TABLE questions (
    id SERIAL PRIMARY KEY,
    title VARCHAR NOT NULL,
    index INT NOT NULL,
    in_quiz_id INT NOT NULL REFERENCES quizzes (id)
);

CREATE TABLE answers (
    id SERIAL PRIMARY KEY,
    title VARCHAR NOT NULL,
    index INT NOT NULL,
    is_correct BOOLEAN NOT NULL,
    question_id INT NOT NULL REFERENCES questions (id)
);

