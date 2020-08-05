function setDataTable(dataUrl, editUrl, columns) {
    if (editUrl) {
        columns.push({
            searchable: false,
            orderable: false,
            render: function (data, type, full, meta) {
                return `<a onClick="editdata('${editUrl}','${full.Id}')" href="javascript://">Edit</a>`;
            }
        })
    }

    return $("#data-table").DataTable({
        // Design Assets
        stateSave: true,
        autoWidth: true,
        // ServerSide Setups
        processing: true,
        serverSide: true,
        // Paging Setups
        paging: true,
        // Searching Setups
        searching: { regex: true },
        ajax: {
            url: dataUrl,
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            data: function (d) {
                return JSON.stringify(d);
            }
        },
        columns,
        order: [[1, 'asc']],
        responsive: true
    });
}

function editdata(url, id) {
    window.location.href = url + `/${id}`
}