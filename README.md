🎭 The ReqnRoll & Playwright "Flash" Framework
"Because 'It works on my machine' is not a valid test status."
Welcome to my flagship automation project! I engineered this suite to solve the three biggest headaches in QA: Tests that take too long, Reports that no one understands, and Bugs that disappear when you try to record them.

I built this from the ground up to be a high-speed, BDD-driven engine that doesn't just "test"—it provides a full forensic audit of your application's health.

🛠️ The Tech Stack (My Strategic Choices)
GitHub's Markdown is great, but for my tech stack, I prefer a bit more "structure." Here is the engine under the hood:

<table width="100%">
<thead>
<tr align="left">
<th width="30%">🚀 Technology</th>
<th width="70%">🎯 My "Why" (The Personality Behind the Choice)</th>
</tr>
</thead>
<tbody>
<tr>
<td><b>.NET 10.0</b></td>
<td>The latest and greatest. If it’s not .NET 10, is it even a modern framework?</td>
</tr>
<tr>
<td><b>ReqnRoll (BDD)</b></td>
<td>The "Translator." It turns <i>"Given I am on the login page"</i> into actual code, so the Product Owner can sleep at night.</td>
</tr>
<tr>
<td><b>Playwright</b></td>
<td>My chosen weapon. It’s like Selenium, but it actually respects your time and doesn't flake out for no reason.</td>
</tr>
<tr>
<td><b>MSTest (Parallelized)</b></td>
<td>I tuned this to run 16 scenarios at once. If your CPU fans aren't spinning, you're not testing hard enough.</td>
</tr>
<tr>
<td><b>ReportPortal & Docker</b></td>
<td>AI-powered dashboards because looking at raw console logs in 2026 is just masochism.</td>
</tr>
<tr>
<td><b>EF Core</b></td>
<td>To keep our database as clean as a whistle. No "ghost data" allowed here.</td>
</tr>
</tbody>
</table>

📸 The "Evidence Locker" (What happens when things break?)
I’ve designed this framework to be a "Crime Scene Investigator." If a test fails (or even when it passes!), it leaves behind a trail of breadcrumbs:

📸 Screenshots: Captured at the exact moment of failure. No more guessing.

🕵️ Traces (Failure Only): I configured Playwright to save full .trace files only on failure. It’s like a black-box flight recorder for your browser—see every click, network request, and console log play-by-play.

📊 Reports: Human-readable LivingDocs that show the "Green vs. Red" status of the business requirements.

📝 Technical Logs: Deep-dive logs (via Serilog) for when you really need to see the Matrix.

📂 Downloads: Testing a "Download PDF" button? My framework actually catches the file and saves it for verification.

📈 My Development Journey (The "Real" Story)
This project was a journey of many coffees and several "What have I done?" moments. Here is the evolution of the codebase:

Phase 1: The Honeymoon: I got ReqnRoll and Playwright to talk to each other. I felt like a god. One test passed. I should have retired then.

Phase 2: The "Spaghetti" Incident: As I added more tests, the code started looking like a bowl of pasta. I spent a week refactoring everything into SOLID principles and a clean Page Object Model. It was painful, but now the code is so clean you could eat off it.

Phase 3: The Need for Speed: Sequential tests are for people with too much free time. I implemented 16-worker parallelism. My computer briefly hovered off the desk, but the test suite finished in record time.

Phase 4: The "Trace" Breakthrough: I realized saving traces for every test was eating my hard drive. I added logic to only save Traces on failure. It’s the "minimalist" approach to debugging.

Phase 5: AI Integration: Bringing in ReportPortal via Docker. Now, the framework uses AI to tell me if a failure is a real bug or just the environment being "moody."

⚙️ Setting Up (The "New Teammate" Guide)
If you've just joined the team (Welcome!), here is how you get this running before your first stand-up:

2. Don't Touch the Brain!
The test data lives in testdata.json. Update your credentials there, but do not change the structure unless you want to see me cry.

3. Choose Your Mode
Mode 1: The "I'm in a hurry" (Smoke tests)
.\run-tests.ps1 -Category "smoke"

Mode 2: The "Warp Speed" (Parallel execution)
.\run-tests.ps1 -Parallel -Workers 16

🎥 Video Session
[Placeholder: A video of me explaining this framework while trying not to look at my own git commit messages from 2 AM]

Final Note: This framework is built for humans. If you're a developer joining the team or a recruiter looking for someone who takes "quality" seriously—this is for you. Let’s build something that doesn't break!
