$(document).ready(async function () {
    const dataUrl = Urls.Artist.GetAll;
    const editUrl = Urls.Artist.Edit;
    const columns = [
        { data: 'Id', visible: false, searchable: false, orderable: false },
        { data: 'Name', name: "Name" }
    ]

    const table = setDataTable('#data-table', dataUrl, editUrl, columns)
})

