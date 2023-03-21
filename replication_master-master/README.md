# Masterless replication in Cassandra

nodetool status

cqlsh

CREATE KEYSPACE tutorial WITH replication = {'class': 'NetworkTopologyStrategy', 'datacenter1': '3'}  AND durable_writes = true;

USE tutorial;

CREATE TABLE posts (
    id int PRIMARY KEY,
    title text,
    content text
);

INSERT INTO posts (id, title, content) VALUES(1,'First post!', 'This is my first post from node1!');

SELECT * FROM tutorial.posts LIMIT 10;

INSERT INTO posts (id, title, content) VALUES(2,'2-nd post!', 'This is my second post from node1!');

INSERT INTO posts (id, title, content) VALUES(3,'3-rd post!', 'This is my third post from node2!');
