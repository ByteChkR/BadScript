# Sum Performance Test

Test: 10 Iterations of the Script

Enabling `BSEngineSettings.ENABLE_CORE_FAST_TRACK` can speed up the code execution because it enables the runtime to skip operator lookups
For Function invocations it will be checked if the invoked object is of type `BSFunction` and if so it will directly call invoke on the object.
For Property and Array Access Expressions it will be checked if the left side is any of the Default Implementations.
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
[BS Benchmark] Execution took: 1945ms (00:00:01.9457422)
[BS Benchmark] Execution took: 1681ms (00:00:01.6811375)
[BS Benchmark] Execution took: 1469ms (00:00:01.4692415)
[BS Benchmark] Execution took: 1462ms (00:00:01.4623518)
[BS Benchmark] Execution took: 1498ms (00:00:01.4981742)
[BS Benchmark] Execution took: 1441ms (00:00:01.4413568)
[BS Benchmark] Execution took: 1408ms (00:00:01.4086262)
[BS Benchmark] Execution took: 1479ms (00:00:01.4790501)
[BS Benchmark] Execution took: 1431ms (00:00:01.4312181)
[BS Benchmark] Execution took: 1421ms (00:00:01.4215207)
```

## Release(BSEngineSettings.ENABLE_CORE_FAST_TRACK = true)
```
[BS Benchmark] Execution took: 1622ms (00:00:01.6220074)
[BS Benchmark] Execution took: 1514ms (00:00:01.5143776)
[BS Benchmark] Execution took: 1367ms (00:00:01.3673465)
[BS Benchmark] Execution took: 1300ms (00:00:01.3005677)
[BS Benchmark] Execution took: 1324ms (00:00:01.3246812)
[BS Benchmark] Execution took: 1292ms (00:00:01.2925829)
[BS Benchmark] Execution took: 1301ms (00:00:01.3013007)
[BS Benchmark] Execution took: 1255ms (00:00:01.2553173)
[BS Benchmark] Execution took: 1283ms (00:00:01.2834546)
[BS Benchmark] Execution took: 1286ms (00:00:01.2860663)
```

## Debug(BSEngineSettings.ENABLE_CORE_FAST_TRACK = false)
```
[BS Benchmark] Execution took: 2288ms (00:00:02.2884674)
[BS Benchmark] Execution took: 2393ms (00:00:02.3938691)
[BS Benchmark] Execution took: 2199ms (00:00:02.1993991)
[BS Benchmark] Execution took: 2185ms (00:00:02.1852352)
[BS Benchmark] Execution took: 2127ms (00:00:02.1270587)
[BS Benchmark] Execution took: 2143ms (00:00:02.1438495)
[BS Benchmark] Execution took: 2182ms (00:00:02.1823951)
[BS Benchmark] Execution took: 2175ms (00:00:02.1753173)
[BS Benchmark] Execution took: 2197ms (00:00:02.1979111)
[BS Benchmark] Execution took: 2064ms (00:00:02.0640460)
```

## Debug(BSEngineSettings.ENABLE_CORE_FAST_TRACK = true)
```
[BS Benchmark] Execution took: 2383ms (00:00:02.3837414)
[BS Benchmark] Execution took: 2232ms (00:00:02.2328067)
[BS Benchmark] Execution took: 2016ms (00:00:02.0165705)
[BS Benchmark] Execution took: 2021ms (00:00:02.0211807)
[BS Benchmark] Execution took: 1992ms (00:00:01.9920701)
[BS Benchmark] Execution took: 1992ms (00:00:01.9925484)
[BS Benchmark] Execution took: 2022ms (00:00:02.0222712)
[BS Benchmark] Execution took: 2024ms (00:00:02.0240630)
[BS Benchmark] Execution took: 2023ms (00:00:02.0231440)
[BS Benchmark] Execution took: 2037ms (00:00:02.0376134)
```