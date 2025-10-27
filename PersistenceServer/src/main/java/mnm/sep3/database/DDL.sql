DROP SCHEMA IF EXISTS proof_of_concept cascade;
CREATE SCHEMA proof_of_concept;
SET SCHEMA 'proof_of_concept';

CREATE TABLE Questions (
                           id SERIAL PRIMARY KEY,
                           question VARCHAR NOT NULL,
                           answer VARCHAR NOT NULL
);
