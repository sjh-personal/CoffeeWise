# CoffeeWise ☕️

CoffeeWise is a web app to help office groups track who’s present for coffee, what everyone ordered, and fairly rotate who pays next—taking into account what everyone’s actually spent and how often they’re present.

---

## How It Works

1. **Mark Who’s In:**  
   Each person checks in if they are in office and want coffee today.

2. **See the Recommendation:**  
   The app suggests who should pay, balancing over time and cost.

3. **Submit the Order:**  
   Enter who paid, what everyone ordered, and the prices.

4. **Check History & Balances:**  
   See past orders and up-to-date balances for the group.

---

## Quick Start
#### Dependencies
- **Docker** and **Docker Compose** (required)
1. **Clone and run with Docker Compose:**
    ```bash
    git clone https://github.com/yourusername/coffeewise.git
    cd coffeewise
    docker compose up --build
    ```
    - Web UI: [http://localhost:5173](http://localhost:5173)
    - API: [http://localhost:5001](http://localhost:5001)
    - API docs: [http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html)

---

## How is Fairness Calculated?

CoffeeWise uses **pairwise balances** to track fairness—more than just a running total. After each order, it keeps a ledger of what each person owes to every other. This works a bit like Splitwise, so balances stay fair even as people come and go, or order different drinks.

**Example:**
- Day 1: Bob, Jeremy, Alice. Bob pays \$9 (\$3 each). Jeremy and Alice each owe Bob $3.
- Day 2: Jeremy, Alice. Jeremy pays \$8 (\$4 each). Alice owes Jeremy \$4. Bob’s balance is unchanged since he was absent.

The app recommends the payer with the lowest net position (most owed) among today’s crew, so everyone gets their turn and nobody falls behind.

---

## Notes & Limitations

- **No authentication** for demo simplicity.
- **Group members** are created/seeded in the initial DB migration; no UI to add or remove people or groups yet.
- **Short polling:** App updates every 5 seconds, so changes by others appear quickly but not instantly.
- **Integrations** like Slack or email notifications were considered, but not included for ease of local demo and review.
- **Extensible:** Architecture is ready for features like Slack, email notifications, or admin tools.

---

*Enjoy your coffee!*
