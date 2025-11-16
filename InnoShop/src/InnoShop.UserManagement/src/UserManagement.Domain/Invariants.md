# Invariants

## User Invariants

*   A User can have only one UserProfile.

## Product Invariants

*   A Product must belong to exactly one Category.
*   A Product is owned by a single User.
*   Products of a deactivated User must be hidden (Soft Delete).

## Review Invariants

*   A User cannot write a review for themselves (the Reviewer's User ID and the Target User's ID cannot be the same).