//Events
$(function () {
    tabSelect('#wikiTab');

    $.connection.chatHub.client.receiveMessage = recMessage;

    $.connection.hub.start().done(function () {
        $('#sendButton').click(function () {
            sendMessage(userData.id, $('#chatText').val());
            $('#chatText').val('');
        });
    });
});

//Tab functionality
function tabSelect(tab) {
    $(".jsTabs").hide();
    $(tab).toggle();
}
function updatePartial(link, contentDiv) {
    $(contentDiv).load(link);
}

//Message sender
function sendMessage(id,message) {
    $.connection.chatHub.server.send(id,message);
}
//Callback
function recMessage(message) {
    var chatMessage = JSON.parse(message);
    
    $(".chatMessages").append(chatMessage.Username + ":" + chatMessage.Message + "<br/>");
}