$(document).ready(function () {
    const movieId = window.location.pathname.split('/').pop()

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
                                <p class="mb-1"><b>${full.UserName}</b> <i class="text-secondary">${full.LocalCreatedAt}</i></p>
                                <p>${full.Text}</p>
                            </div>`;
                }
            }
        ],
        responsive: true
    });

    $('#comment-create-btn').on('click', async function () {
        const commentText = $('#comment-create-form').find('textarea[name=Text]').val()
        if (!commentText || commentText.length < 1)
            alert('Enter the comment text')
        else {
            await createComment(movieId, commentText)

            $('#comment-create-form').trigger('reset')
            table.ajax.reload()
        }
    })

})

async function createComment(movieId, text) {
    try {
        const data = {
            movieId,
            text
        }

        await $.ajax({
            type: "POST",
            url: Urls.Comment.Create,
            data: JSON.stringify(data),
            contentType: 'application/json; charset=utf-8',
            cache: false,
            timeout: 600000
        });
        
    } catch (err) {
        console.error(err);
    }
}