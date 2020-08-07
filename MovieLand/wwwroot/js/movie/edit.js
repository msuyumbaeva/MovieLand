$(document).ready(async function () {
    var movieId = $('input[name=Id]').val()

    // Init genre choices
    const genreChoices = new ChoicesClient('#genresSelectList')
    genreChoices.handleChange()
    genreChoices.handleSearch(Urls.Genre.GetAll)
    genreChoices.handleAddItem()
    genreChoices.handleRemoveItem()
    genreChoices.initChoices(`${Urls.Movie.GetGenres}/${movieId}`)

    // Init country choices
    var countryChoices = new ChoicesClient('#countriesSelectList')
    countryChoices.handleSearch(Urls.Country.GetAll)
    countryChoices.handleChange()
    countryChoices.handleAddItem()
    countryChoices.handleRemoveItem()
    countryChoices.initChoices(`${Urls.Movie.GetCountries}/${movieId}`)

    // Init director choices
    var directorChoices = new ChoicesClient('#directorsSelectList')
    directorChoices.handleSearch(Urls.Artist.GetAll)
    directorChoices.handleChange()
    directorChoices.handleAddItem()
    directorChoices.handleRemoveItem()
    directorChoices.initChoices(`${Urls.Movie.GetArtists}?id=${movieId}&career=Director`)

    // Init actor choices
    var actorChoices = new ChoicesClient('#actorsSelectList')
    actorChoices.handleSearch(Urls.Artist.GetAll)
    actorChoices.handleChange()
    actorChoices.handleAddItem()
    actorChoices.handleRemoveItem()
    actorChoices.initChoices(`${Urls.Movie.GetArtists}?id=${movieId}&career=Actor`)

})