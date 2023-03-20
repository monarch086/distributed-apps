# Sync Replication Example

## Create and setup containers

docker network create my-net

docker run --network my-net --name pmaster -p 5432:5432 -v ./pmaster_data:/var/lib/postgresql/data -e POSTGRES_PASSWORD=postgres -d postgres

docker run --network my-net --name pstandby -p 5433:5432 -v ./pstandby_data:/var/lib/postgresql/data -e POSTGRES_PASSWORD=postgres -d postgres

docker stop pmaster pstandby

### This is not correct way of copying database - it will throw errors, pg_basebackup should be used instead (see below)

copy -R master_data pstandby_data

docker start pmaster pstandby

docker exec -it pmaster psql -U postgres
select * from pg_stat_replication;

### Correct way of copying master database

`CREATE USER replicator WITH REPLICATION ENCRYPTED PASSWORD 'postgres';
SELECT * FROM pg_create_physical_replication_slot('replication_slot_slave1');
SELECT * FROM pg_replication_slots;`

`docker exec -it pmaster pg_basebackup -D /tmp/postgresslave -S replication_slot_slave1 -X stream -P -U replicator -Fp -R`

- copy backup files to pstandby container

### Setup master

- in 'pmaster_data/postgres.conf':
  - add 'synchronous_standby_names = 'first 1 (standby1)'';

- in 'pmaster_data/pg_hba.conf':
  - add 'host replication postgres all md5';

### Setup standby

- in 'pmaster_data/postgres.conf':
  - add 'primary_conninfo = 'application_name=standby1 host=pmaster port=5432 user=postgres password=postgres'';

- in 'pmaster_data/pg_hba.conf':
  - add 'host replication postgres all md5';

## Run

docker-compose up

## Check replication

SELECT * FROM pg_replication_slots;
SELECT * FROM pg_stat_replication;

## Check operations

`docker exec -it pmaster psql -U postgres
create table test(id int, t varchar(200));`

`docker exec -it pstandby psql -U postgres
\d test;`

insert into test (id, t) values (1, 'data 1');
select * from test;
