//Events
$(function () {
    $('#wikiTab').toggle();

    $.connection.chatHub.client.receiveMessage = recMessage;

    $.connection.hub.start().done(function () {
        $('#sendButton').click(function () {
            sendMessage(userData.id, $('#chatText').val());
            $('#chatText').val('');
        });
    });
});
//Tab functionality
function tabSelect(sender,tab) {
    $(".jsTabs").hide();
    $(".selectedtab").removeClass("selectedtab");

    $(sender).addClass("selectedtab");
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
    //TODO: Add message coloring
    //TODO: Handle private messages
    var chatMessage = JSON.parse(message);

    var mClass = getMessageClass(chatMessage.MessageType);
    
    $(".chatMessages").append("<span class='"+mClass+"'>"+chatMessage.Username + ":" + chatMessage.Message + "</span><br/>");
}
function getMessageClass(type) {
    switch (type) {
        case "StandardMessage":
            return "";
        case "Whisper":
            return "whisper";
        case "Roll":
            return "roll";
    }
}