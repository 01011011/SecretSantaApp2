$(function () {

    var UserModel = function (data) {

        data = data || {};
        var self = this;

        this.Id = ko.observable(data.Id);
        this.Name = ko.observable(data.Name);
        this.inGroup = ko.observable(false);
        this.checked = ko.observable(false);
        this.Match = ko.observable(data.Match);
        this.showCheckbox = ko.observable(true);
    };

    var GroupModel = function (data) {

        data = data || {};
        this.Id = ko.observable(data.Id);
        this.Name = ko.observable(data.Name);
        this.Users = ko.observableArray(data.Users);
    };

    var MatchViewModel = function(data) {
        var self = this;
        self.Sender = ko.observable(data.Sender);
        self.Receiver = ko.observable(data.Receiver);
    };

    var SecretSantaViewModel = function (users) {
        
        var self = this;
        this.baseUriUsers = "api/Users";
        this.baseUriGroups = "api/Groups";

        this.users = ko.observableArray([]);
        this.selectedUsers = ko.observableArray([]);
        this.groups = ko.observableArray([]);
        this.userToAdd = ko.observable("");
        this.groupName = ko.observable("");
        this.usersInGroups = ko.observableArray([]);
 
        this.matches = ko.observableArray([]);
        this.showMatching = ko.pureComputed(function() {
            return self.matches().length > 0;
        });

        this.flatten = function (a) {
            return Array.isArray(a) ? [].concat.apply([], a.map(self.flatten)) : a;
        };

        this.usersIdsInGroups = ko.pureComputed(function () {
            var idArray = self.flatten(self.usersInGroups());
            var result = idArray.map(a => a.Id);
            return result;
        });

        this.checkVisible = ko.pureComputed(function () {
            return self.users().length > 1;
        });

        this.enableGroupAdd = ko.pureComputed(function() {
            return self.selectedUsers().length > 1;
        });

        this.usersVisible = ko.pureComputed(function() {
            return self.users().length > 0;
        });

        this.groupsVisible = ko.pureComputed(function () {
            return self.groups().length > 0;
        });

        this.canAddToGroup = ko.pureComputed(function() {
            return self.selectedUsers().length > 0 && self.groupsVisible();
        });

        this.showBoth = ko.pureComputed(function() {
            return self.canAddToGroup() && self.enableGroupAdd();
        });

        this.selectedGroup = ko.observable();

        this.updateGroup = function() {
            self.updateExistingGroup(this.selectedGroup().Id(), this.selectedUsers());
        };

        this.mapUserData = function (data) {

            self.users(ko.utils.arrayMap(data, function (d) {

                var model = new UserModel(d);

                var match = ko.utils.arrayFirst(self.selectedUsers(), function (i) {
                    return d.Id === i.Id();
                });

                if (match) {
                    model.checked = true;
                }

                var showCheck = ko.utils.arrayFirst(self.usersIdsInGroups(), function (i) {
                    return d.Id === i;
                });

                if (showCheck) {
                    model.inGroup = true;
                    model.showCheckbox = false;
                }

                return model;
            }));
        };

        this.mapGroupData = function (data) {
            self.groups(ko.utils.arrayMap(data, function (d) {
                self.usersInGroups.push(d.Users);
                return new GroupModel(d);
            }));
        };

        this.getUserData = function() {
            $.ajax({
                url: self.baseUriUsers,
                type: 'GET',
                accepts: "application/json",
                contentType: "application/json"
            }).done(function (result) {
                self.users.removeAll();
                if (result === undefined || result === null) {
                    return;
                } else {
                    if (result.length > 0) {
                        self.mapUserData(result);
                    }
                }
            }).fail(function (xhr, error, status) {
                alert("Status Code: " + status +"\nMessage: " + xhr.responseText);
            });
        };

        this.addUser = function() {

            if (self.userToAdd() !== "") {

                $.ajax({
                    url: self.baseUriUsers,
                    type: 'POST',
                    accepts: "application/json",
                    data: JSON.stringify(self.userToAdd()),
                    contentType: "application/json"
                }).done(function(result) {
                    self.getUserData();
                    self.userToAdd("");
                }).fail(function(xhr, error, status) {
                    if (status === "Conflict") {
                        alert(xhr.responseJSON.Message);
                    } else {
                        alert("Status Code: " + status + "\nMessage: " + xhr.responseText);
                    }
                });
            }
        };

        this.delete = function (user) {

            self.removeIfSelected(user);

            $.ajax({
                url: self.baseUriUsers + "/?id=" + user.Id(),
                type: 'DELETE'
            }).done(function(result) {
                self.getUserData();
            }).fail(function(xhr, error, status) {
                if (status === "NotFound") {
                    alert(xhr.responseJSON.Message);
                } else {
                    alert("Status Code: " + status + "\nMessage: " + xhr.responseText);
                }
            });
        };

        this.removeIfSelected = function (user) {
            
            var match = ko.utils.arrayFirst(self.selectedUsers(), function (i) {
                return user.Id() === i.Id();
            });

            if (match) {
                self.selectedUsers.remove(match);
            }
        };

        this.removeAllSelectedUsers = function () {
            self.selectedUsers.removeAll();
        };

        this.userChecked = function (user, element) {
            var $checkBox = $(element.target);
            var isChecked = $checkBox.is(":checked");

            //If it is checked and not in the array, add it
            if (isChecked && self.selectedUsers().indexOf(user) < 0) {
                self.selectedUsers.push(user);
            }
            //If it is in the array and not checked remove it                
            else if (!isChecked && self.selectedUsers().indexOf(user) >= 0) {
                self.selectedUsers.remove(user);
            }
            //Need to return to to allow the Checkbox to process checked/unchecked
            return true;
        };
        
        this.newGroup = function () {
            $("#group-name-submit").show();
        };

        this.canRemoveUserFromGroup = function(id) {
            //implement
            var flatGroup = self.flatten(self.groups());
            var retItem = ko.toJS(flatGroup).filter(function (item) {
                return item.Id === parseInt(id);
            });
            var numberOfUsers = retItem[0].Users.length;
            
            return numberOfUsers > 2;
        };

        this.deleteGroup = function(id, user) {
            $.ajax({
                url: self.baseUriGroups + "/?id=" + id,
                type: 'DELETE',
                traditional: true,
                data: JSON.stringify(user),
                contentType: "application/json"
            }).done(function (result) {
                self.getGroupData();
                self.getUserData();
            }).fail(function (xhr, error, status) {
                if (status === "NotFound") {
                    alert(xhr.responseJSON.Message);
                } else {
                    alert("Status Code: " + status + "\nMessage: " + xhr.responseText);
                }
            });
        };

        this.removeFromGroup = function (user, event) {

            var groupId = $(event.target).closest("tr").find("input[name='group-id']").val();

            if (!self.canRemoveUserFromGroup(groupId)) {
                var deleteWholeGroup = confirm("Each group has to have at least 2 people.  Deleting " +
                    "this item will delete the whole group.  Are you sure you want to continue?");

                if (deleteWholeGroup) {
                    self.deleteGroup(groupId, user);
                }
            } else {
                self.deleteGroup(groupId, user);
            }
        };

        this.updateExistingGroup = function (id, users) {

            $.ajax({
                url: self.baseUriGroups + "/?id=" + id,
                type: 'PUT',
                traditional: true,
                data: JSON.stringify(ko.toJS(users)),
                contentType: "application/json"
            }).done(function (result) {
                self.groups.removeAll();
                self.groupName("");
                self.removeAllSelectedUsers();
                self.getGroupData();
                self.getUserData();
            }).fail(function (xhr, error, status) {
                if (status === "Conflict") {
                    alert(xhr.responseJSON.Message);
                } else {
                    alert("Status Code: " + status + "\nMessage: " + xhr.responseText);
                }
            });
        };

        this.saveGroup = function () {

            var group = { Id: null, Name: self.groupName(), Users: ko.toJS(self.selectedUsers) };

            $.ajax({
                url: self.baseUriGroups,
                type: 'POST',
                traditional: true,
                data: JSON.stringify(group),
                contentType: "application/json"
            }).done(function (result) {
                self.groups.removeAll();
                self.groupName("");
                self.removeAllSelectedUsers();
                self.getGroupData();
                self.getUserData();
            }).fail(function (xhr, error, status) {
                if (status === "Conflict") {
                    alert(xhr.responseJSON.Message);
                } else {
                    alert("Status Code: " + status + "\nMessage: " + xhr.responseText);
                }
            });
        };

        this.runAlghoritm = function () {

            $.ajax({
                url: self.baseUriUsers + "/Match",
                type: 'GET',
                traditional: true,
                contentType: "application/json"
            }).done(function (result) {

                if (result.length == 0) {
                    alert("Not possible matching!!!");
                } else {
                    self.matches.removeAll();
                    self.matches(ko.utils.arrayMap(result, function (d) {
                        return new MatchViewModel(d);
                    }));
                }
            }).fail(function(xhr, error, status) {
                if (status === "Conflict") {
                    alert(xhr.responseJSON.Message);
                } else {
                    alert("Status Code: " + status + "\nMessage: " + xhr.responseText);
                }
            });
        };

        this.getGroupData = function () {
            $.ajax({
                url: self.baseUriGroups,
                type: 'GET',
                accepts: "application/json",
                contentType: "application/json"
            }).done(function (result) {
                if (result === undefined || result === null) {
                    return;
                } else {
                    self.usersInGroups.removeAll();
                    self.groups.removeAll();
                    if (result.length >= 0) {
                        self.mapGroupData(result);
                    }
                }
            }).fail(function (xhr, error, status) {
                alert("Status Code: " + status + "\nMessage: " + xhr.responseText);
                });
        };

        this.getGroupData();
        this.getUserData();
    };

    ko.applyBindings(new SecretSantaViewModel());
});