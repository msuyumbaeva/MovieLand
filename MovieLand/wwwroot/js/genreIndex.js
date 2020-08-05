$(document).ready(async function () {
    const dataUrl = '/Genre/LoadTable';    
    const editUrl = '/Genre/Edit';
    const columns = [
        { data: 'Id', visible: false, searchable: false, orderable: false },
        { data: 'Name', name: "Name" }
    ]

    const table = setDataTable(dataUrl, editUrl, columns)
})

