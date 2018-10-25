"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/timerHub").build();

connection.on("ReceiveMessage", function (response) {
    console.log(response);
    $('#update-timer').text(response);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});