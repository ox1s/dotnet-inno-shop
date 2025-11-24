# 📖 Ubiquitous Language / Единый Язык

## 👤 User Context (User Management)

### **User / Пользователь**
*   <span>&#x1F1FA;&#x1F1F8;</span> A registered person in the system. The core entity (Aggregate Root).
    *   **Identity:** Uniquely identified by an ID.
    *   **Role:** Can be a standard `User` or an `Admin`.
    *   **State:** Can be `Active` or `Deactivated` (Banned).
    *   **Capabilities:** Can create `Products` (only if they have a `UserProfile`) and write `Reviews`.
*   <span>&#x1F1F7;&#x1F1FA;</span> Зарегистрированный человек в системе. Основная сущность (Корень Агрегата).
    *   **Идентификация:** Уникально определяется по ID.
    *   **Роль:** Может быть обычным `Пользователем` или `Админом`.
    *   **Состояние:** Может быть `Активен` или `Деактивирован` (Забанен).
    *   **Возможности:** Может создавать `Продукты` (только при наличии `Профиля`) и писать `Отзывы`.

### **UserProfile / Профиль Пользователя**
*   <span>&#x1F1FA;&#x1F1F8;</span> The public face of a `User`. Contains detailed information (Avatar, Phone, Bio) visible to other buyers.
    *   **Requirement:** A `User` **must** create a `UserProfile` before they can post any `Products`. A `UserProfile` must be from Belarus.
    *   **Relationship:** Strictly one-to-one with a `User`.
*   <span>&#x1F1F7;&#x1F1FA;</span> Публичное лицо `Пользователя`. Содержит детальную информацию (Аватар, Телефон, О себе), видимую покупателям.
    *   **Требование:** `Пользователь` **обязан** создать `Профиль`, прежде чем сможет выкладывать `Продукты`.
    *   **Связь:** Строго один-к-одному с `Пользователем`.

### **Review / Отзыв**

* 🇺🇸 **Feedback left by one `User` (the Reviewer) regarding another `User` (the Seller).**
    * **Components:** Includes a rating (1–5 stars) and a text comment.
    * **Constraint:** A `User` cannot write a `Review` for themselves.
    * **Relation to UserProfile:**  
        A `Review` can be created **only by a User who has a valid `UserProfile`**, and can be left **only for a User who also has a `UserProfile`**.  
        The `UserProfile` represents the public identity (name, avatar, etc.), which is the actual subject of user feedback.  

* 🇷🇺 **Обратная связь, оставленная одним `Пользователем` (Ревьюером) другому `Пользователю` (Продавцу).**
    * **Состав:** Включает рейтинг (1–5 звёзд) и текстовый комментарий.
    * **Ограничение:** `Пользователь` не может написать `Отзыв` самому себе.
    * **Связь с UserProfile:**  
        Отзыв может оставить только тот пользователь, у которого есть заполненный `UserProfile`, и получить отзыв тоже может только пользователь с `UserProfile`.  
        `UserProfile` отражает публичную личность пользователя (имя, аватар и т. д.), к которой фактически и относится обратная связь.  


### **Admin / Админ**
*   <span>&#x1F1FA;&#x1F1F8;</span> A `User` with elevated privileges responsible for platform moderation.
    *   **Authority:** Can `Deactivate` users who violate rules.
*   <span>&#x1F1F7;&#x1F1FA;</span> `Пользователь` с расширенными правами, ответственный за модерацию платформы.
    *   **Полномочия:** Может `Деактивировать` пользователей, нарушающих правила.

### **Deactivate (Ban) / Деактивация (Бан)**
*   <span>&#x1F1FA;&#x1F1F8;</span> An action performed by an `Admin` to suspend a `User`'s access.
    *   **Side Effect:** When a `User` is deactivated, a domain event is triggered to **Hide** all their `Products` in the Product Catalog.
*   <span>&#x1F1F7;&#x1F1FA;</span> Действие, выполняемое `Админом` для приостановки доступа `Пользователя`.
    *   **Побочный эффект:** Когда `Пользователь` деактивирован, срабатывает доменное событие, которое **Скрывает** все его `Продукты` в каталоге.

---

## 📦 Product Context (Product Management)

### **Product / Продукт (Товар)**
*   <span>&#x1F1FA;&#x1F1F8;</span> An item listed for sale (Aggregate Root).
    *   **Ownership:** Owned by a single `User` (the Seller).
    *   **Visibility:** Can be `Visible` or `Hidden` (Soft Deleted).
    *   **Data:** Contains a `SellerSnapshot` to display seller info quickly.
    *   **Audit:** Must track the Creation Date (`CreatedAt`) to sort items by novelty. 
*   <span>&#x1F1F7;&#x1F1FA;</span> Предмет, выставленный на продажу (Корень Агрегата).
    *   **Владение:** Принадлежит одному `Пользователю` (Продавцу).
    *   **Видимость:** Может быть `Видимым` или `Скрытым` (Мягкое удаление).
    *   **Данные:** Содержит `Слепок Продавца` для быстрого отображения информации о владельце.
    *   **Аудит:** Необходимо отслеживать дату создания (`CreatedAt`) для сортировки элементов по новизне.

### **SellerSnapshot / Слепок Продавца**
*   <span>&#x1F1FA;&#x1F1F8;</span> A read-only copy (Value Object) of the Seller's essential info (Name, Avatar, Rating) stored directly within the `Product`.
    *   **Purpose:** Allows displaying the product card without querying the User Context.
    *   **Sync:** Updated automatically via events when the `User` updates their `UserProfile`.
*   <span>&#x1F1F7;&#x1F1FA;</span> Доступная только для чтения копия (Объект-значение) основной информации о Продавце (Имя, Аватар, Рейтинг), хранящаяся прямо в `Продукте`.
    *   **Цель:** Позволяет отображать карточку товара без запросов в Контекст Пользователя.
    *   **Синхронизация:** Обновляется автоматически через события, когда `Пользователь` меняет свой `Профиль`.

### **Category / Категория**
*   <span>&#x1F1FA;&#x1F1F8;</span> A classification group for `Products` (e.g., "Phones", "Laptops"). Used for filtering the catalog.
*   <span>&#x1F1F7;&#x1F1FA;</span> Группа классификации для `Продуктов` (например, "Телефоны", "Ноутбуки"). Используется для фильтрации каталога.

### **Wishlist / Избранное**
*   <span>&#x1F1FA;&#x1F1F8;</span> A personal collection of `Products` that a `User` has marked as "Favorite" for future reference.
*   <span>&#x1F1F7;&#x1F1FA;</span> Личная коллекция `Продуктов`, которые `Пользователь` отметил как "Любимые" для быстрого доступа.

### **Soft Delete (Hide) / Мягкое Удаление (Скрытие)**
*   <span>&#x1F1FA;&#x1F1F8;</span> The technical mechanism for removing a `Product` from the public catalog without erasing data from the database.
    *   **Triggers:** Can be triggered by the Seller (deleting their own product) or automatically by the System (when the Seller is `Deactivated`).
*   <span>&#x1F1F7;&#x1F1FA;</span> Технический механизм удаления `Продукта` из публичного каталога без физического стирания данных из базы.
    *   **Триггеры:** Может быть вызвано Продавцом (удаление своего товара) или автоматически Системой (когда Продавец `Деактивирован`).