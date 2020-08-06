$(document).ready(async function () {
    const dataUrl = Urls.Country.GetAll;
    const editUrl = Urls.Country.Edit;
    const columns = [
        { data: 'Id', visible: false, searchable: false, orderable: false },
        { data: 'Name', name: "Name" }
    ]

    const table = setDataTable('#data-table', dataUrl, editUrl, columns)
})

