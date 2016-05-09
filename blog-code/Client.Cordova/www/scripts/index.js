(function () {
    "use strict";

    var client,         // Connection to the Azure Mobile App backend
        origClient,     // We filter the client eventually - this is a copy of the original
        dataTable,      // Reference to the table endpoint on backend
        pushService;

    var azureBackend = 'https://shellmonger-demo.azurewebsites.net';

    document.addEventListener('deviceready', onDeviceReady.bind(this), false);

    /**
     * Event handler, called when the host is ready to process requests
     * Note: DO NOT USE PLUGINS UNTIL THIS POINT
     * @event
     */
    function onDeviceReady() {
        console.info('Creating Connection to Backend ' + azureBackend);
        client = new WindowsAzure.MobileServiceClient(azureBackend);
        dataTable = client.getTable('TodoItem');
        $('#loginButton').on('click', function (event) {
            event.preventDefault();
            client.login('aad').then(initializeApp, function (error) {
                console.error('Authentication Failed: ', error);
                alert('Authentication Failed');
            });
        });
    }

    /**
     * Initializes the app
     */
    function initializeApp() {
        console.info('client.currentUser:', client.currentUser);
        $('#wrapper').empty();

        // Replace the wrapper with the main content for the task list
        var content =
            '<div id="wrapper">'
          + '<article>'
          + '  <header>'
          + '    <div id="title"><h2>Azure</h2><h1>Mobile Apps</h1></div>'
          + '    <div id="add-item">'
          + '      <button id="refresh-data">Refresh</button>'
          + '      <button id="add">Add</button>'
          + '      <div><input type="text" id="new-item-text" placeholder="Enter new task" /></div>'
          + '    </div>'
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
        $('#refresh-data').on('click', handleRefresh);

        // Initialize Push Notifications
        if (typeof PushNotification === 'undefined') {
            alert('Push Notifications are not available');
        } else {
            pushService = PushNotification.init({
                android: { senderID: '121035973492' },
                ios: { alert: 'true', badge: 'true', sound: 'true' },
                wns: {}
            });
            pushService.on('registration', handlePushRegistration);
            pushService.on('notification', handlePushNotification);
            pushService.on('error', handleError);
        }
    }

    /**
     * Event Handler for response from PNS registration
     * @param {object} data the response from the PNS
     * @param {string} data.registrationId the registration Id from the PNS
     * @event
     */
    function handlePushRegistration(data) {
        var apiOptions = {
            method: 'POST',
            body: {
                pushChannel: data.registrationId,
                tags: ['News', 'Sports', 'Politics', '$email:myboss@microsoft.com' ]
            }
        };

        var success = function () {
            //alert('Push Registered');
        }
        var failure = function (error) {
            alert('Push Registration Failed: ' + error.message);
        }

        client.invokeApi("register", apiOptions).then(success, failure);
    }

    /**
     * Event Handler for receiving a push notification
     * @param {object} data the payload from the PNS
     * @param {string} data.message the textual message
     * @event
     */
    function handlePushNotification(data) {
        alert('PUSH NOTIFICATION\n' + data.message);
    }

    /**
     * Event Handler for the Refresh Button
     * @param {object} event the event that caused the call
     * @event
     */
    function handleRefresh(event) {
        event.preventDefault();
        refreshDisplay();
    }

    /**
     * Refresh the data from Azure
     */
    function refreshDisplay() {
        updateSummaryMessage('Loading Data from Azure');
        dataTable.where({ complete: false }).read().then(updateTaskList, handleError);
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
        updateSummaryMessage('Error Occurred');
        $('#errorlog').append($('<li>').text(text));
    }


    /**
     * Create the <LI> element for the task
     * @param {object} item the Item used for creating the list element
     * @returns {jQuery} the list item
     */
    function createTask(item) {
        console.info('Creating task for item', item);
        return $('<li>')
            .attr('data-todoitem-id', item.id)
            .append($('<button class="item-delete">Delete</button>'))
            .append($('<input type="checkbox" class="item-complete">').prop('checked', item.complete))
            .append($('<div>').append($('<input class="item-text">').val(item.title)));
    }

    /**
     * Updates the list based on the response from azure
     * @param {object[]} items the list of items returned from azure
     */
    function updateTaskList(items) {
        console.info('Items received:', items);
        var listItems = $.map(items, createTask);
        $('#todo-items').empty().append(listItems).toggle(listItems.length > 0);
        $('#summary').html('<strong>' + items.length + '</strong> item(s)');

        console.info('Installing event handlers on the item-complete checkboxes');
        $('.item-complete').on('change', updateItemCompleteHandler);

        console.info('Installing event handlers on the item-text boxes');
        // For when the focus changes
        $('.item-text').on('change', updateItemTextHandler);
        // For when the enter key is pressed
        $('.item-text').on('keydown', function (event) {
            if (event.which === 13) {
                updateItemTextHandler(event);
            }
        });

        console.info('Installing event handlers on the delete buttons');
        $('.item-delete').on('click', deleteItemHandler);
    }

    /**
     * Returns the item id based on the reference in the list
     * @param {DOMElement} el the element to reference
     * @returns {string} the ID of the element
     */
    function getItemId(el) {
        console.info('[getItemId]: EL =', el);
        var id = $(el).closest('li').attr('data-todoitem-id');
        console.info('[getItemId]: ID =', id);
        return id;
    }

    /**
     * Add Item Handler
     * @param {object} event the event that caused the call
     * @event
     */
    function addItemHandler(event) {
        var textbox = $('#new-item-text'),
            itemText = textbox.val();

        console.info('[addItemHandler] itemText =', itemText);
        if (itemText !== '') {
            updateSummaryMessage('Adding New Item');
            dataTable.insert({ title: itemText, complete: false }).then(function (item) {
                console.info('[addItemHandler] item = ', item);
                updateSummaryMessage('Item added in Azure');
                var listItem = createTask(item);
                $('#todo-items').append(listItem);
            }, handleError);
        }
        textbox.val('').focus();
        event.preventDefault();
    }

    /**
     * Delete Item Handler
     * @param {object} event the event that caused the call
     * @event
     */
    function deleteItemHandler(event) {
        var id = getItemId(event.currentTarget);

        console.info('[deleteItemHandler] id =', id);
        updateSummaryMessage('Deleting Item');
        dataTable.del({ id: id }).then(function () {
            $(event.currentTarget).closest('li').remove();
            updateSummaryMessage('Item deleted in Azure');
        }, handleError);
        event.preventDefault();
    }

    /**
     * Update Textbox Handler
     * @param {object} event the event that caused the call
     * @event
     */
    function updateItemTextHandler(event) {
        var id = getItemId(event.currentTarget),
            newText = $(event.currentTarget).val();

        console.info('[updateItemTextHandler] id = ' + id + ', newText = ' + newText);
        updateSummaryMessage('Updating title field');
        dataTable.update({ id: id, title: newText }).then(function (item) {
            console.info('[updateItemTextHandler] item = ', item);
            updateSummaryMessage('Item is updated in Azure');
        }, handleError);
        event.preventDefault();
    }

    /**
     * Update Completed Item Handler
     * @param {object} event the event that caused the call
     * @event
     */
    function updateItemCompleteHandler(event) {
        var id = getItemId(event.currentTarget),
            isComplete = $(event.currentTarget).prop('checked');

        console.info('[updateItemCompleteHandler] id = ' + id + ', isComplete = ' + isComplete);
        updateSummaryMessage('Updating Item');
        dataTable.update({ id: id, complete: isComplete }).then(function (item) {
            console.info('[updateItemCompleteHandler] item = ', item);
            updateSummaryMessage('Item is updated in Azure');
        }, handleError);
        event.preventDefault();
    }
})();