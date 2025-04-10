mergeInto(LibraryManager.library, {
    SyncFiles: function() {
        FS.syncfs(false, function (err) {
            if (err) {
                console.error('Error synchronizing file system:', err);
            } else {
                console.log('File system synchronized successfully.');
            }
        });
    }
});