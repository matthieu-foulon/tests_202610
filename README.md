folder description :

test_20260930 : contains implementation of all 4 questions
BenchProject  : contains some performance benchmark
TestProject   : contains unit tests

in test_20260930/interfaces, I gathered interface as close as possible from the spec.


in test_20260930/q1, I tried 2 implementations as answer :

- ProbabilisticRandomGenImplBasic, as the name implies,
  a basic algorithm where at each iteration I draw a random single and determine its position
  in a sorted list of accumulated probability, so a little expensive each time ;

- ProbabilisticRandomGenImpl, a more direct access one, at construction I build an array of "number" where
  the probability to draw an index of the array if the one given in NumAndProbability.
  so the cost of computation is minimal but the storage and its access can be a problem given
  what deviation of the real probabilities we can accept (an more little array grant better access but
  is less precise).

for both, I didn't really care about multithreading, since there is no sharing of resource in write mode.


in test_20260930/q2, I implmented an event bus relying on a string topic to route events.

published events are stored in a BlockingCollection which handles synchronization and reading in a parallel matter.
subscribers are stored by topic in a class which allow us, in some cases, to filter duplicates.


in test_20260930/q3, again 2 kinds :

- ThrottlerImpl, token bucket.

- ThrottlerSlidingWindow, sliding window.

only point I don't understand is the push event :
- through a event bus, I won't be able to distribute the token because I don't know the subscriber count ;
- if I handle subscibers myself, it just multiple the risk a burst


in test_20260930/q4, one implementation which accumulates integers and where we can ask for statistics at
any time.