$(document).ready(async function () {
    // Init genre choices
    var genreChoices = new ChoicesClient('#genresSelectList')
    genreChoices.handleSearch(Urls.Genre.GetAll)
    genreChoices.handleChange()

    // Init country choices
    var countryChoices = new ChoicesClient('#countriesSelectList')
    countryChoices.handleSearch(Urls.Country.GetAll)
    countryChoices.handleChange()

    // Init director choices
    var directorChoices = new ChoicesClient('#directorsSelectList')
    directorChoices.handleSearch(Urls.Artist.GetAll)
    directorChoices.handleChange()

    // Init actor choices
    var actorChoices = new ChoicesClient('#actorsSelectList')
    actorChoices.handleSearch(Urls.Artist.GetAll)
    actorChoices.handleChange()
})

