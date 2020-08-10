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

    // Delete items on button click
    $('#movie-edit-btn').on('click', function () {
        // Delete genres
        genreChoices.removes.forEach(g => {
            deleteData(`${Urls.Movie.DeleteGenre}?movieId=${movieId}&genreId=${g}`)
        })

        // Delete countries
        countryChoices.removes.forEach(c => {
            deleteData(`${Urls.Movie.DeleteCountry}?movieId=${movieId}&countryId=${c}`)
        })

        // Delete directors
        directorChoices.removes.forEach(d => {
            deleteData(`${Urls.Movie.DeleteArtist}?movieId=${movieId}&artistId=${d}&career=Director`)
        })

        // Delete actors
        actorChoices.removes.forEach(a => {
            deleteData(`${Urls.Movie.DeleteArtist}?movieId=${movieId}&artistId=${a}&career=Actor`)
        })
        $('#movie-edit-form').submit()
    })

})

async function deleteData(deleteUrl) {
    try {
        await $.ajax({
            type: "DELETE",
            url: deleteUrl,
            contentType: 'application/json; charset=utf-8',
            cache: false,
            timeout: 600000
        });
    } catch (err) {
        console.error(err);
    }
}