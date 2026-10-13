Folder Description

- test_20260930: contains implementation of all 4 questions
- BenchProject: contains some performance benchmark
- TestProject: contains unit tests
In test_20260930/interfaces, I gathered interface as close as possible from the spec.

Question 1 (test_20260930/q1)

I tried 2 implementations as answer:
- ProbabilisticRandomGenImplBasic: as the name implies, a basic algorithm where at each iteration I draw a random single and determine its position in a sorted list of accumulated probability, so a little expensive each time ;
- ProbabilisticRandomGenImpl: a more direct access one, at construction I build an array of "number" where the probability to draw an index of the array if the one given in NumAndProbability. so the cost of computation is minimal but the storage and its access can be a problem given what deviation of the real probabilities we can accept (a more little array grants better access but is less precise).

For both, I didn't do much about multithreading, since there is no sharing of resource in write mode.

Question 2 (test_20260930/q2)

I implemented an event bus relying on a string topic to route events.
Published events are stored in a BlockingCollection which handles synchronization and reading in a parallel manner. Subscribers are stored by topic in a class which allow us, in some cases, to filter duplicates.

Question 3 (test_20260930/q3)

Again 2 kinds:
- ThrottlerImpl: token bucket. warning: I forgot the limitation in time.
- ThrottlerSlidingWindow: sliding window.
Both use a thread for bucket filling or clean old time.

Only point I don't understand is the push event:
- Through a event bus, I won't be able to distribute the token because I don't know the subscriber count ;
- If I handle subscribers myself, it just multiple the risk of a burst.

Question 4 (test_20260930/q4)

One implementation which accumulates integers and where we can ask for statistics at any time.
Handles multiple clients through the event bus of q2, since the data of the event are for reading, there is no data to protect in a multi thread context.


time taken : approx. 2 days
