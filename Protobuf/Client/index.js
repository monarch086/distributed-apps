const request = require('request');

request('http://127.0.0.1:5098/employees', { json: true }, (err, res, body) => {
  if (err) { return console.log(err); }

  console.table(body);
});

request('http://127.0.0.1:5098/employees/proto', { json: true }, (err, res, body) => {
  if (err) { return console.log(err); }

  console.table(body);
});