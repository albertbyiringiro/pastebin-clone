# Backlog

WIP limit: 1. Expand a milestone only after the previous gate passes.

Status: TODO / DOING / DONE / PARKED

---

## M0 — See the bytes  (target: 2 sessions)

### M0-1 · Watch a raw HTTP request arrive
Status: DOING
Size: S (30 min)
Why: Everything above this is an abstraction over text on a socket. You
     should see the text once.
Do:  `nc -l 8080` in one terminal, `curl -v http://localhost:8080/hi?x=1`
     in another. Then repeat with a POST and a JSON body.
Done when: You can write out, from memory, the four parts of an HTTP
     request and say which line the router cares about.
Learn: request line, headers, blank line, body. Why `Content-Length`
     exists.
Trap: Skipping this because "I already know HTTP." You know it as a
     concept, not as bytes.

### M0-2 · Serve HTTP with no framework
Size: M (2-3 h)
Why: Kestrel stops being magic once you've written the bad version.
Do:  A console app using TcpListener that accepts a connection, reads the
     request, prints it, and writes a hardcoded 200 with a JSON body.
     Every line typed, no copy-paste.
Done when: A browser renders your JSON, and you can explain why the blank
     line before the body is load-bearing.
Learn: sockets, streams, the response status line, Content-Length.
Trap: Forgetting `\r\n\r\n`. Good — spend ten minutes confused.

### M0-3 · Break it on purpose
Size: S (45 min)
Why: Concurrency is the fifth primitive and your server doesn't have it.
Do:  Hit M0-2 with two curls at once. Then send a request in two chunks
     (`nc` + slow typing) and watch the single ReadAsync fail.
Done when: You can name three defects in your own server: one connection
     at a time, assumes the whole request arrived, no routing.
Learn: why a thread-per-request model exists, why parsing is stateful.
Trap: Fixing them. Don't. Write them in LEARNING.md and move on.

### M0-4 · Gate → M1
Run `/gate`. Then expand M1 below if it needs adjusting.

---

## M1 — A paste bin on the public internet  (target: 4 weeks)

### M1-1 · Hello JSON
Size: S (45 min)
Why: Smallest possible closed loop. Project to running route.
Do:  `dotnet new web -o src/Shortly`. One `MapGet`. Return an object.
     Nullable reference types ON, warnings as errors in the .csproj.
Done when: `curl localhost:5000/ping` returns JSON, and the build is
     clean with warnings-as-errors.
Learn: the `dotnet` CLI, the minimal Program.cs, implicit usings.
Trap: Adding folders. One file. Resist.

### M1-2 · Own your Program.cs
Size: S (1 h, spread over 3 days)
Why: The fingers rule. This is the file you'll edit a hundred times.
Do:  Delete Program.cs and retype it from a blank file. Do this three
     times across three days. Use `/explain` for any line you can't
     justify.
Done when: You can type a working Program.cs with a POST and a GET from
     memory, nothing open, under 5 minutes.
Learn: WebApplicationBuilder vs WebApplication, what `builder.Build()`
     actually returns, where the pipeline begins.
Trap: Counting "I read it carefully" as doing this.

### M1-3 · SQL by hand
Size: M (3 h)
Why: Learn the floor before the ORM, so EF Core later reads as
     translation rather than magic.
Do:  SQLite via Microsoft.Data.Sqlite. Write CREATE TABLE yourself.
     Parameterized INSERT and SELECT. No Dapper yet, no EF.
Done when: You can insert and read a paste row from a `dotnet run`
     route, and you can explain why the query is parameterized.
Learn: connections, commands, readers, SQL injection as a consequence of
     the trust boundary.
Trap: Reaching for an ORM the moment mapping gets tedious. The tedium is
     the lesson — it's the pain EF Core was invented to pay for.

### M1-4 · POST /pastes
Size: M (2-3 h)
Why: First real write path.
Do:  Accept `{ content, title?, language? }`. Store it. Return 201 with a
     `Location` header and the short code.
Done when: `curl -i -X POST` shows 201 and a Location you can follow.
Learn: model binding, `Results.Created`, why 201 + Location rather than
     200 + body.
Trap: Returning 200. Status codes are the API's grammar; get them right
     now, not later.

### M1-5 · GET /{code}
Size: S (1-2 h)
Why: Closes the loop. Now it's a product.
Do:  Return the paste as JSON. 404 when it doesn't exist.
Done when: Post a paste, follow the Location, get it back. Hit a garbage
     code, get a clean 404 with no stack trace.
Learn: route parameters, `Results.NotFound`, the difference between
     "missing" and "error".
Trap: Leaking an exception page. Check what a bad code returns in
     Production environment, not just Development.

### M1-6 · Short codes that don't collide
Size: M (2 h)
Why: First genuinely interesting design decision in the project.
Do:  Pick a scheme (random base62? counter + encoding?). Handle the
     collision case explicitly.
Done when: You can state your collision probability and show what the
     code does when one happens — retry, or 409.
Learn: unique constraints as the enforcement point, why "check then
     insert" is a race and the DB constraint isn't.
Trap: Checking for existence in C# before inserting. Two requests, same
     millisecond. Write that test.

### M1-7 · The input is hostile
Size: M (2 h)
Why: Pillar six. Your endpoint is open to the internet in two tickets.
Do:  Max content size, reject empty, length-cap title and language,
     validate the code format on GET. Return 422 or 400 deliberately —
     decide which and be consistent.
Done when: A 50MB POST is rejected before it's buffered, and a request
     with `title` of 10,000 chars gets a clean 400/422 with a useful
     message.
Learn: request size limits, where validation belongs, 400 vs 422.
Trap: Validating in three places. Pick one.

### M1-8 · Configuration and secrets
Size: S (1 h)
Why: You're about to deploy; this is the ticket where a connection string
     gets committed if you skip it.
Do:  Connection string from appsettings.json, overridden by an
     environment variable. Nothing sensitive in git. Add .gitignore.
Done when: `ASPNETCORE_ENVIRONMENT=Production` plus an env var runs
     against a different database, with no code change.
Learn: the configuration provider chain and its precedence order.
Trap: appsettings.Development.json committed with real values.

### M1-9 · A Dockerfile you wrote
Size: M (2-3 h)
Why: Deployment stops being a black box.
Do:  Multi-stage: SDK image builds, runtime image runs. Typed, not
     generated, not AI-written.
Done when: `docker run -p 8080:8080` serves your API, and the image is
     under ~250MB.
Learn: build vs runtime images, why the SDK doesn't ship to production,
     what `EXPOSE` does and doesn't do.
Trap: Copying a Dockerfile from a blog. Type it and know each line.

### M1-10 · Ship it
Size: M (2-4 h)
Why: A localhost project is not shipped. This is the ticket that makes
     the previous nine real.
Do:  A $5 VPS, or Fly.io / Railway. HTTPS. A domain or a subdomain.
Done when: You send someone a URL, they paste text, they get a link back
     that works on their phone.
Learn: ports and binding in a container, reverse proxies, where logs go
     when there's no terminal.
Trap: Binding to localhost inside the container. It'll work locally and
     fail in deploy, and that's a good hour to spend.

### M1-11 · Post-ship retro
Size: S (30 min)
Why: The sediment step.
Do:  Append to LEARNING.md: what surprised you, what you had to look up
     more than twice, what you'd do differently.
Done when: Your confusion log has at least five honest entries.

### M1-12 · Gate → M2
Run `/gate`. Explain a request end to end out loud. Then expand M2.

---

## Not yet expanded

- **M2** — Expiry, /raw, correct status codes everywhere, first
  integration tests over WebApplicationFactory.
- **M3** — Port the data layer to EF Core, diff its generated SQL against
  the SQL you wrote in M1-3. Accounts and auth. Rate limiting.
- **M4** — Burn-after-read (discover why it's a concurrency problem,
  don't be told), password-protected pastes, caching.
- **M5** — Observability, a load test, Postgres, extract a reusable
  template.

Do not write these as tickets yet.
