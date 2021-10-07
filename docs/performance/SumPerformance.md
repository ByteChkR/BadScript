# Sum Performance Test

Test: 10 Iterations of the Script

Enabling `BSEngineSettings.ENABLE_CORE_FAST_TRACK` can speed up the code execution because it enables the runtime to skip operator lookups
For Function invocations it will be checked if the invoked object is of type `BSFunction` and if so it will directly call invoke on the object.
For Property and Array Access Expressions it will be checked if the left side is the `BSArray` implementations.
For `BSObject`s that have `BSObject.IsLiteral` set to true, all custom operator lookups will be skipped.

## Performance Test Script
```js
function sumIfEven(current, i)
{
	mod = i % 2
	if(mod == 0)
	{
		return current + i
	}
	return current
}

function sumEven(count)
{
	s = 0
	for i = 0 while< count
	{
		s = sumIfEven(s, i)
	}
	return s
}

sumEven(100000)
```

## Release(BSEngineSettings.ENABLE_CORE_FAST_TRACK = false)

```
[BS Benchmark] Execution took: 1763ms (00:00:01.7636590)
[BS Benchmark] Execution took: 1572ms (00:00:01.5721414)
[BS Benchmark] Execution took: 1416ms (00:00:01.4162247)
[BS Benchmark] Execution took: 1410ms (00:00:01.4101174)
[BS Benchmark] Execution took: 1392ms (00:00:01.3925059)
[BS Benchmark] Execution took: 1416ms (00:00:01.4164204)
[BS Benchmark] Execution took: 1398ms (00:00:01.3986590)
[BS Benchmark] Execution took: 1345ms (00:00:01.3456606)
[BS Benchmark] Execution took: 1393ms (00:00:01.3939583)
[BS Benchmark] Execution took: 1424ms (00:00:01.4242693)
```

## Release(BSEngineSettings.ENABLE_CORE_FAST_TRACK = true)
```
[BS Benchmark] Execution took: 1942ms (00:00:01.9426940)
[BS Benchmark] Execution took: 1519ms (00:00:01.5190239)
[BS Benchmark] Execution took: 1537ms (00:00:01.5370845)
[BS Benchmark] Execution took: 1354ms (00:00:01.3543053)
[BS Benchmark] Execution took: 1370ms (00:00:01.3703933)
[BS Benchmark] Execution took: 1356ms (00:00:01.3562920)
[BS Benchmark] Execution took: 1434ms (00:00:01.4340419)
[BS Benchmark] Execution took: 1285ms (00:00:01.2853741)
[BS Benchmark] Execution took: 1347ms (00:00:01.3470740)
[BS Benchmark] Execution took: 1337ms (00:00:01.3376131)
```

## Debug(BSEngineSettings.ENABLE_CORE_FAST_TRACK = false)
```
[BS Benchmark] Execution took: 2573ms (00:00:02.5738851)
[BS Benchmark] Execution took: 2310ms (00:00:02.3109708)
[BS Benchmark] Execution took: 2179ms (00:00:02.1799220)
[BS Benchmark] Execution took: 2145ms (00:00:02.1453585)
[BS Benchmark] Execution took: 2100ms (00:00:02.1000010)
[BS Benchmark] Execution took: 2116ms (00:00:02.1166006)
[BS Benchmark] Execution took: 2134ms (00:00:02.1341559)
[BS Benchmark] Execution took: 2156ms (00:00:02.1568945)
[BS Benchmark] Execution took: 2148ms (00:00:02.1482774)
[BS Benchmark] Execution took: 2037ms (00:00:02.0374560)
```

## Debug(BSEngineSettings.ENABLE_CORE_FAST_TRACK = true)
```
[BS Benchmark] Execution took: 2443ms (00:00:02.4439287)
[BS Benchmark] Execution took: 2271ms (00:00:02.2713900)
[BS Benchmark] Execution took: 2185ms (00:00:02.1853650)
[BS Benchmark] Execution took: 2131ms (00:00:02.1312689)
[BS Benchmark] Execution took: 2069ms (00:00:02.0696931)
[BS Benchmark] Execution took: 2115ms (00:00:02.1152066)
[BS Benchmark] Execution took: 2212ms (00:00:02.2129762)
[BS Benchmark] Execution took: 2195ms (00:00:02.1954239)
[BS Benchmark] Execution took: 2141ms (00:00:02.1412750)
[BS Benchmark] Execution took: 2047ms (00:00:02.0473923)
```