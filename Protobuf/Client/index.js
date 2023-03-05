const request = require('request');
const Schema = require('./employees_pb');

const host = 'http://127.0.0.1:5098';

request(`${host}/employees`, { json: true }, (err, res, body) => {
  if (err) { return console.log(err); }

  console.table(body);
});

request(`${host}/employees/proto`, { json: true }, (err, res, body) => {
  if (err) { return console.log(err); }

  console.log('Encoded protobuf data: ', body);
});

request(`${host}/employees/proto`, { json: true }, (err, res, body) => {
  if (err) { return console.log(err); }

  const employees = Schema.Employees.deserializeBinary(body);

  console.log('Decoded protobuf data: ');
  console.table(employees.toObject().employeesList);
});