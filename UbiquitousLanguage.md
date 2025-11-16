# 📖 Ubiquitous Language - InnoShop Project


### **User**

*   A registered person in the system. A `User` is the core entity that interacts with the application
    *   Can have a role, such as a customer or an `Admin`
    *   Can have only one `UserProfile`
    *   Can create `Products` and write `Reviews`

### **UserProfile**

*   Contains detailed personal information about a `User` that is visible to others in the system
    *   A `UserProfile` is created by a `User` to gain the ability to post `Products`
    *   It has a strict one-to-one relationship with a `User`

### **Product**

*   An item for sale created by a `User`
    *   Each `Product` is owned by a single `User`
    *   Must be assigned to exactly one `Category`
    *   Can be hidden from view using `Soft Delete`

### **Category**

*   A classification for a `Product`, used for grouping and filtering

### **Review**

*   Feedback from one `User` (the `Reviewer`) to another (the `Target User`), which includes a rating and a comment
    *   A `User` cannot write a `Review` for themselves

### **Admin**

*   A `User` with a special role that grants privileges to manage other `Users` and system content
    *   Performs `Activate / Deactivate` actions on `User` accounts
    *   Moderates content such as `Products` and `Reviews`

### **Activate / Deactivate**

*   The actions an `Admin` performs on a `User`'s account to manage their access and visibility within the system.
*   Deactivating a `User` must hide all of their `Products` from public view.

### **Soft Delete**

*   A mechanism for hiding a `Product` from public view without permanently deleting it from the database.
*   This is used when a `User` is `Deactivated`, allowing their `Products` to be easily restored if the `User` is `Activated` again.