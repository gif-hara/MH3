mergeInto(LibraryManager.library, {
	_SyncFile: function() {
		FS.syncfs(false, function (err) {
			console.log('Error: syncfs failed!');
		});
	},
});
