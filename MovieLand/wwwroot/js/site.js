// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const formatArrayForChoices = function (array) {
    const newArray = new Array()
    array.map(a => {
        newArray.push({
            "value": a["id"],
            "label": a["name"]
        })
    })
    return newArray;
}
