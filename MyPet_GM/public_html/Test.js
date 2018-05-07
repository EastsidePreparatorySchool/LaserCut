/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */


function print(text) {
    document.getElementById("output").innerHTML += text;
}


function printline(text) {
    document.getElementById("output").innerHTML += text + "<br>";
}


function test () {
    printline("Hello world!");
}

console.log("Hello")