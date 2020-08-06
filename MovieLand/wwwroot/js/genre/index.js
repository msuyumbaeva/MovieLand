$(document).ready(async function () {
    const dataUrl = Urls.Genre.GetAll;
    const editUrl = Urls.Genre.Edit;
    const columns = [
        { data: 'Id', visible: false, searchable: false, orderable: false },
        { data: 'Name', name: "Name" }
    ]

    const table = setDataTable('#data-table', dataUrl, editUrl, columns)
})

