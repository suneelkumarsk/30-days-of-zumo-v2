(function () {
    "use strict";

    var client,         // Connection to the Azure Mobile App backend
        dataTable;      // Reference to the table endpoint on backend

    document.addEventListener( 'deviceready', onDeviceReady.bind(this), false);

    /**
     * Event handler, called when the host is ready to process requests
     * Note: DO NOT USE PLUGINS UNTIL THIS POINT
     * @event
     */
    function onDeviceReady() {
        client = new WindowsAzure.MobileServiceClient('https://shell-4ca00908-6291-4333-8894-0ffd5291b110.azurewebsites.net');
        dataTable = client.getTable('TodoItem');

        $('#loginButton').on('click', function (event) {
            event.preventDefault();

            client.login('aad').then(initializeApp, function (error) {
                console.error('Authentication Failed: ', error);
                alert('Authentication Failed');
            });
        });
    }

    function initializeApp() {
        console.info('client.currentUser:', client.currentUser);
        $('#wrapper').empty();

        // Replace the wrapper with the main content for the task list
        var content =
            '<div id="wrapper">'
          + '<article>'
          + '  <header>'
          + '    <div id="title"><h2>Azure</h2><h1>Mobile Apps</h1></div>'
          + '    <form id="add-item">'
          + '      <button id="refresh">Refresh</button>'
          + '      <button id="add">Add</button>'
          + '      <div><input type="text" id="new-item-text" placeholder="Enter new task" /></div>'
          + '    </form>'
          + '  </header>'
          + '  <ul id="todo-items"></ul>'
          + '  <p id="summary">Initializing...</p>'
          + '</article>'
          + '<footer><ul id="errorlog"></ul></footer>'
          + '</div>';
        $("#page").html(content);

        refreshDisplay();

        // Wire up the event handlers
        $('#add').on('click', addItemHandler);
        $('#refrresh').on('click', handleRefresh);
    }

    /**
     * Event Handler for the Refresh Button
     * @event
     */
    function handleRefresh(event) {
        updateSummaryMessage('Loading Data from Azure');
        dataTable.where({ complete: false }).read().then(updateTaskList, handleError);
        event.preventDefault();
    }

    /**
     * Update the summary message
     * @param {string} msg the message to display
     */
    function updateSummaryMessage(msg) {
        $('#summary').html(msg);
    }

    /**
     * Update the error list
     * @param {Error} error the thrown error
     */
    function handleError(error) {
        var text = error + (error.request ? ' - ' + error.request.status : '');
        console.error(text);
        $('#errorlog').append($('<li>').text(text));
    }


    /**
     * Create the <LI> element for the task
     * @param {object} item the Item used for creating the list element
     * @returns {jQuery} the list item
     */
    function createTask(item) {
        return $('<li>')
            .attr('data-todoitem-id', item.id)
            .append($('<button class="item-delete">Delete</button>'))
            .append($('<input type="checkbox" class="item-complete">').prop('checked', item.complete))
            .append($('<div>').append($('<input class="item-text">').val(item.text)));
    }

    /**
     * Updates the list based on the response from azure
     * @param {object[]} items the list of items returned from azure
     */
    function updateTaskList(items) {
        var listItems = $.map(items, createTask);
        $('#todo-items').empty().append(listItems).toggle(listItems.length > 0);
        $('#summary').html('<strong>' + items.length + '</strong> item(s)');

        $('.item-delete').on('click', deleteItemHandler);
        $('.item-text').on('change', updateItemTextHandler);
        $('.item-complete').on('change', updateItemCompleteHandler);
    }

    /**
     * Returns the item id based on the reference in the list
     * @param {DOMElement} el the element to reference
     * @returns {string} the ID of the element
     */
    function getItemId(el) {
        return $(el).closest('li').attr('data-todoitem-id');
    }

    /**
     * Add Item Handler
     * @event
     */
    function addItemHandler(event) {
        event.preventDefault();

        var textbox = $('#new-item-text'),
            itemText = textbox.val();

        updateSummaryMessage('Adding New Item');
        if (itemText != '') {
            dataTable.insert({ text: itemText, complete: false }).then(function (item) {
                console.info('new item = ', item);
                updateSummaryMessage('Item added in Azure');
                var listItem = createTask(item);
                $('#todo-items').append(listItem);
            }, handleError);
        }

        textbox.val('').focus();
    }

    /**
     * Delete Item Handler
     * @event
     */
    function deleteItemHAndler(event) {
        event.preventDefault();

        var id = getItemId(event.currentTarget);
        updateSummaryMessage('Deleting Item');
        dataTable.del({ id: id }).then(function () {
            $(event.currentTarget).closest('li').remove();
            updateSummaryMessage('Item deleted in Azure');
        }, handleError);
    }

    /**
     * Update Textbox Handler
     * @event
     */
    function updateItemTextHandler(event) {
        event.preventDefault();

        var id = getItemId(event.currentTarget),
            newText = $(event.currentTarget).val();
        updateSummaryMessage('Updating text field');
        dataTable.update({ id: id, text: newText }).then(function () {
            updateSummaryMessage('Item is updated in Azure');
        }, handleError);
    }

    /**
     * Update Completed Item Handler
     * @event
     */
    function updateItemCompleteHandler(event) {
        event.preventDefault();

        var id = getItemId(),
            isComplete = $(event.currentTarget).prop('checked');

        updateSummaryMessage('Updating Item');
        dataTable.update({ id: id, complete: isComplete }).then(function () {
            updateSummaryMessage('Item is updated in Azure');
        }, handleError);
    }
})();