var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();



connection.on("RecieveMessage", function (username, message) {
    const msg = username + ": " + message;
    const li = document.createElement("li");
    li.textContent = msg;
    document.getElementById("list").appendChild(li);
});

document.getElementById("btnSend").addEventListener("click", event => {
    // const username = document.getElementById("txtUser").value;
    const receiver = document.getElementById("receiveUser").value;
    const message = document.getElementById("txtMsg").value;
    connection.invoke("SendMessagetoGroup", receiver, message).catch(err => console.error(err.toString()));
    event.preventDefault();
})
connection.start().catch(err => console.error(err.toString()));
