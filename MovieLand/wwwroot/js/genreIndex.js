$(document).ready(async function () {
    const dataUrl = genreLoadTable;
    const editUrl = genreEdit;
    const columns = [
        { data: 'Id', visible: false, searchable: false, orderable: false },
        { data: 'Name', name: "Name" }
    ]

    const table = setDataTable(dataUrl, editUrl, columns)
})

