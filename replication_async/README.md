# Async Replication Example

## Create and setup containers

docker-compose up

### Setup master

docker exec -it postgres_primary psql -U postgres -W postgres

CREATE ROLE replicator WITH REPLICATION PASSWORD 'postgres' LOGIN;

SELECT * FROM pg_create_physical_replication_slot('replication_slot_slave1');

SELECT * FROM pg_replication_slots;

pg_basebackup -D /tmp/postgresslave -S replication_slot_slave1 -X stream -P -U replicator -Fp -R

- copy backup files to pg_secondary container

- in 'primary/data/pg_hba.conf':
  - add:
   'host    replication     replicator      all            trust
    host    replication     replicator      all            md5';

### Setup standby

- in 'secondary/data/postgresql.auto.conf':
  - add:
   'primary_conninfo = 'host=pg_primary port=5432 user=replicator password=postgres'
    primary_slot_name = 'replication_slot_slave1'
    restore_command = 'cp /var/lib/postgresql/data/pg_wal/%f "%p"'';

## Run

docker-compose up

## Check replication

SELECT * FROM pg_replication_slots;
SELECT * FROM pg_stat_replication;

## Check operations

`docker exec -it postgres_primary psql -U postgres
create table test(id int, t varchar(200));`

`docker exec -it postgres_secondary psql -U postgres
\d test;`

insert into test (id, t) values (1, 'data 1');
select * from test;
