const express = require('express');
const app = express();
const fs = require('fs');
const ejs = require('ejs');
const mysql = require('mysql');


const client = mysql.createConnection({
    host: "localhost",
    user: "root",
    password: "1234",
    database: "smart_farm",
    port: 3307
});
const mainPage = fs.readFileSync('./index.ejs', 'utf8');
 
app.get('/', (req, res) => {
    var page = ejs.render(mainPage, {
        title: "Smart Farm",
    });
    res.send(page);
});
 
app.get('/getdata?', (req, res) => {
    
    client.query("SELECT * FROM smart_farm;", function(err, result, fields){
        if(err) throw err;
        else{
            var page = ejs.render(mainPage, {
                title: "Smart_farm",
                data: result,
            });
            res.send(page);
        }
    });
});
 
app.listen(8000, () => {
    console.log('Server Running on Port 8000!');
});