$(document).ready(async function () {
    // Get movie id from url
    const movieId = window.location.pathname.split('/').pop()
    const movieName = $('#movie-name').text()

    // Setup datatable plugin for movie comments
    var table = $('#comments-table').DataTable({
        // Language Setups
        language: {
            zeroRecords: "No comments"
        },
        // Design Assets
        stateSave: true,
        autoWidth: true,
        // ServerSide Setups
        processing: true,
        serverSide: true,
        // Paging Setups
        paging: true,
        // Searching Setups
        searching: false,
        // Ordering Setups
        ordering: false,
        // Info Setups
        info: false,
        // Page length Setups
        lengthChange: false,
        pageLength: 5,
        ajax: {
            url: `${Urls.Comment.GetAll}/${movieId}`,
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            data: function (d) {
                return JSON.stringify(d);
            }
        },
        columns: [
            {
                data: "Name",
                name: "Name",
                render: function (data, type, full, meta) {
                    return `<div">
                                <p class="mb-1"><b>${full.User}</b> <i class="text-secondary">${full.LocalCreatedAt}</i></p>
                                <p>${full.Text}</p>
                            </div>`;
                }
            }
        ],
        responsive: true
    });

    // Get avg rate of movie
    let ratingValue = $('#star-rating-average').html()

    // Flag to prevent sending initial rating value 
    let initialRating = ratingValue > 0

    // Setup rating plugin for movie rating bar
    $('#star-rating').rating({
        // Value setup
        value: ratingValue,
        // Click handler
        click: async function (e) {
            // If not initial value
            if (initialRating) {
                initialRating = false
                return
            }
            // Get value
            const value = e.stars;
            // Post request
            await postData(Urls.StarRating.Create, { movieId, value })
            await SendRatingToHub(movieName, value)

            // Reload avg rate
            await loadRatingAverage(movieId)
        }
    });

    // #comment-create-btn click handler
    // Creates new comment, reloads form and comments table
    $('#comment-create-btn').on('click', async function () {
        const commentText = $('#comment-create-form').find('textarea[name=Text]').val()
        if (!commentText || commentText.length < 1)
            alert('Enter the comment text')
        else {
            await postData(Urls.Comment.Create, { movieId, text: commentText })
            // Reset form
            $('#comment-create-form').trigger('reset')
            // Reload comments table
            table.ajax.reload()
        }
    })      
})

async function loadRatingAverage(movieId) {
    const rating = await getData(`${Urls.StarRating.GetAverageRateOfMovie}/${movieId}`)
    $('#star-rating-average').html(rating.Entity)
    return rating.Entity
}



