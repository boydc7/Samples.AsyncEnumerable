A simple, inconclusive, unscientific, not-covering-real-world scenarios test of how IAsyncEnumerable performs against a syncronous enumerable and batched-async fetch of similar data in CPU-bound only operations.

I was mostly curious here as to how a straight IAsyncEnumerable<T> operation performed in comparison to an IAsyncEnumerable<IEnumerable<T>> operation when dealing with a remote data source where the amount of data/records you are allowed to fetch is limited per call (i.e. think of something like a scan against AWS DyanmoDb, where the # of rows/amount of data you can retrieve on a single call).  My theory was that you were likely to get better performance in an environment like that yielding out batches of records fetched vs. flatting those batches.

In this very simlple, very naive test...it doesn't seem to matter, they perform similarly. 
