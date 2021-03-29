// <copyright file=JobTitles.cs/>
// <copyright>
//   Copyright (c) 2018, Affective & Cognitive Institute
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software andassociated documentation files
//   (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify,
//   merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//   OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//   LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
//   IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// <license>MIT License</license>
// <main contributors>
//   Moritz Umfahrer
// </main contributors>
// <co-contributors/>
// <patent information/>
// <date>07/12/2018 05:59</date>

using System.Collections.Generic;

/// <summary>
///     WARNING! THE CURRENT STATE OF THIS CLASS IS WORK IN PROGRESS.
///     IN THE FIRST STEP WE WANT TO CREATE A DEMONSTRATION (THAT'S WHAT THIS SCRIPT IS FOR)
///     IN THE SECOND STEP WE SHOULD PROVIDE A FUNCTIONALITY TO DEFINE JOBS AND LOAD THEM DYNAMICALLY FROM THE DATABASE
/// </summary>
public class JobTitles
{
    public static Dictionary<int, string> JOBS;

    public static void InitializeJobs()
    {
        if (JOBS == null)
            JOBS = new Dictionary<int, string>();
        else
            return;

        JOBS.Add((int) Jobs.JobOne, "Job One");
        JOBS.Add((int) Jobs.JobTwo, "Job Two");
        JOBS.Add((int) Jobs.JobThree, "Job Three");
        JOBS.Add((int) Jobs.JobFour, "Job Four");
    }
}

public enum Jobs
{
    JobOne = 0,
    JobTwo,
    JobThree,
    JobFour
}