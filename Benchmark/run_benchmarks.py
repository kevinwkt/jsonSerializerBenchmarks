import json
import os
import time


def clean_state():
	os.system('cmd /c "taskkill /F /IM dotnet.exe /T > nul 2>&1"')
	# time.sleep(5)


processes = ["Serialize", "Deserialize"]
types = ["LoginViewModel", "Location", "IndexViewModel"]
mechanisms = ["SourceGeneration", "System.Text.Json",
    "Json.NET", "Utf8Json", "Jil"]

clean_state()

results = {}


def run_new_benchmarks(output_path):
	new_results = {}
	min_results = {}

	for process in processes:
		for t in types:
			for mechanism in mechanisms:
				job = "{} {} {}".format(process, mechanism, t)
				print("Running {}".format(job))

				elasped_times = []

				for i in range(4):
					os.system(
					    'cmd /c "dotnet publish -c Release -p:PublishReadyToRun=true --runtime win-x64 > nul 2>&1"')
					print("[CMD]: bin\\Release\\netcoreapp5.0\\win-x64\\publish\\Benchmark.exe %s %s %s " %
					      (process, mechanism, t))
                    # Move generated crossgen DLL into published app.
					os.system('cmd /c "copy /y C:\\Users\\t-kywo\\Documents\\System.Text.Json.dll Benchmark\\bin\\Release\\netcoreapp5.0\\win-x64 > nul 2>&1"')
					os.system('cmd /c "copy /y C:\\Users\\t-kywo\\Documents\\System.Text.Json.dll Benchmark\\bin\\Release\\netcoreapp5.0\\win-x64\\publish > nul 2>&1"')
					elasped_time = os.popen(
					    'cmd /c "bin\Release\\netcoreapp5.0\\win-x64\\publish\\Benchmark.exe {} {} {}"'.format(process, mechanism, t)).read().strip()
					print("{} us".format(elasped_time))

					elasped_times.append(int(elasped_time))
					clean_state()

					if job in min_results:
						min_results[job] = min(min_results[job], int(elasped_time)) 
					else:
						min_results[job] = int(elasped_time)

				average = sum(elasped_times) / len(elasped_times)

				new_results[job] = average
				print("Average: {} us".format(average))
	
	with open(output_path, "w") as results_file:
		json.dump(new_results, results_file)
		json.dump(min_results, results_file)
	
	return new_results

def load_results(input_path):
	new_results = {}
	with open(input_path) as results_file:
		new_results = json.load(results_file)

	return new_results


# results = load_results('start_up_results_hardcode_json.json')
results = run_new_benchmarks('throughput_results_source_generation.json')
print(results)

print("Summary\n=======\n")

for process in processes:
	for t in types:
		max_average = max([results["{} {} {}".format(process, mechanism, t)] for mechanism in mechanisms])

		test_col_values = [len(x) for x in mechanisms]
		test_col_values.append(len("Test"))

		test_col_width = max(test_col_values)
		mean_col_width = max(len("Mean (us)"), len(str(max_average)))
		ratio_col_width = len("Ratio")

		# Write header
		header = "| {} | {} | {} |".format(
			"Test" + "".join([" "] * (test_col_width - len("Test"))),
			"Mean (us)" + "".join([" "] * (mean_col_width - len("Mean (us)"))),
			"Ratio")

		header_underline = "|{}|{}|{}|".format(
			"".join(["-"] * (test_col_width + 2)),
			"".join(["-"] * (mean_col_width + 2)),
			"".join(["-"] * (ratio_col_width + 2)))

		print(header)
		print(header_underline)

		for mechanism in mechanisms:
			# Baseline is default System.Text.Json run without options
			baseline = results["{} System.Text.Json {}".format(process, t)]
			
			mean = results["{} {} {}".format(process, mechanism, t)]
			mean_as_str = str(mean)

			ratio_as_str = format(mean / float(baseline), '.2f')
			
			print("| {} | {} | {} |".format(
				mechanism + "".join([" "] * (test_col_width - len(mechanism))),
				mean_as_str + "".join([" "] * (mean_col_width - len(mean_as_str))),
				ratio_as_str + "".join([" "] * (ratio_col_width - len(ratio_as_str)))
			))

		print()
