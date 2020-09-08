var connection = new signalR.HubConnectionBuilder().withUrl("/ratingHub").build();
var chartBlock = '\u25A3'; //(U+25A3) is "▣" 

connection.on("ReceiveMessage", function (user, movieId, movieValue) {
    var ratingResultMsg = user + " rated '" + movieId + "' : " + movieValue + " .";

    var notifList = $('#notification-list');

    var notifToAdd = $('<a>', {
        class: 'dropdown-item',
        href: '#',
        text: ratingResultMsg
    })

    var notifBadge = $('#notification-badge');
    var notifCount = notifBadge.text() == '' ? 0 : Number.parseInt(notifBadge.text());
    notifCount = notifCount + 1;
    notifBadge.text(notifCount);

    // append to top
    notifList.prepend(notifToAdd)
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

const SendRatingToHub = (movieId, movieValue) => {
    connection.invoke("SendMessage", movieId, movieValue).catch(function (err) {
        return console.error(err.toString());
    });
}