$(document).ready(async function () {
    const dataUrl = Urls.Movie.GetAll;
    const editUrl = null;
    const columns = [
        {
            data: "Name",
            name: "Name",
            render: function (data, type, full, meta) {
                return `<div class="d-inline-flex">
                            <img src="${Urls.RelativeRoot}/posters/${full.Poster}" width="58" height="84" class="mr-3" />
                            <div class="movie-about">
                                <p class="movie-name mb-1"><a href="${Urls.Movie.Details}/${full.Id}">${full.Name}</a></p>
                                <span class="text-secondary">${full.OriginalName}, ${full.Duration} min</span>
                            </div>
                        </div>`;
            }
        },
        {
            data: "ReleaseYear",
            name: "ReleaseYear"
        }
    ]
    const order = [0, 'asc']

    const table = setDataTable('#data-table', dataUrl, editUrl, columns, order)
})

function details(id) {
    window.location.href = Urls.Movie.Details + `/${id}`
}

