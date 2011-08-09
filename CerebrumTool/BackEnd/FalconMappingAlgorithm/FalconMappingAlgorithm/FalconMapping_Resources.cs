/******************************************************************** 
 * Cerebrum Embedded System Design Automation Framework
 * Copyright (C) 2010  The Pennsylvania State University
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 ********************************************************************/
/******************************************************************** 
 * FalconMapping_Resources.cs
 * Name: Matthew Cotter
 * Date: 23 Mar 2010 
 * Description: This class implements resource Scoring for use during the Component-to-FPGA mapping algorithm.
 * >> ( 7 Oct 2010) Matthew Cotter: Moved ResourceInfo and ResourceSet classes to FalconResources project to be more widely available.
 *                                  Changes required due to migration of ResourceInfo & ResourceSet structures to separate library.
 *                                  Converted generic Hashtables to TypedHashtables<> for readability and verifiability.
 * >> (15 Jun 2010) Matthew Cotter: Modified resource management/verification to be case-insensitive -- All resources are compared as lower-case strings.
 *                                  Corrected bug in resource scoring when a requirement of 0 was scored against a non-existant resource.
 * >> (12 May 2010 Matthew Cotter): Renamed file from ResourceInfo.cs to FalconMapping_Resources.cs
 * >>                               Moved FalconMapping_ResourceScoring Class to this file from FalconMapping_Algorithm.cs
 * >> (29 Mar 2010) Matthew Cotter: ResourceSet Class created.
 * >> (29 Mar 2010) Matthew Cotter: ResourceSet Class created.
 * >> (23 Mar 2010) Matthew Cotter: ResourceInfo Class created.
 * >> (23 Mar 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using CerebrumSharedClasses;
using FalconResources;

namespace FalconMappingAlgorithm
{
    /// <summary>
    /// Static class of functions for resource scoring.
    /// </summary>
    internal static class FalconMapping_ResourceScoring
    {
        /// <summary>
        /// Scores a set of resources against a baseline set of resources.
        /// </summary>
        /// <param name="htBaselineSet">The hashtable set of resources to use as a baseline for scoring.</param>
        /// <param name="htCompareSet">The hashtable set of resources to score against the baseline.</param>
        /// <returns>A value indicating how well the comparison set of resources fits into the baseline set, or -1.0 if 
        /// the baseline set cannot accomodate the comparison set.</returns>
        internal static double ScoreResourceSet(Dictionary<string, long> htBaselineSet, Dictionary<string, long> htCompareSet)
        {
            double score = 0.0F;
            long iResCount = 0;

            foreach (string resCompare in htCompareSet.Keys)
            {
                long iComp = (long)htCompareSet[resCompare];
                if (htBaselineSet.ContainsKey(resCompare))
                {
                    long iBase = (long)htBaselineSet[resCompare];
                    double resScore;
                    if (iBase == 0)
                        resScore = 0;
                    else
                        resScore = (double)((double)iComp / (double)iBase);

                    // Lower score is better here, lower score values
                    // indicate that the available resources far exceed
                    // those required, whereas higher values mean
                    // the available resources is not as plentiful.

                    score += resScore;
                    iResCount++;
                }
                else
                {
                    // Return -1 to indicate that this FPGA cannot support 
                    // the required resources
                    if (iComp > 0)
                        return -1.0F;
                }
            }

            if (iResCount == 0)
                return 0;

            // Average the score over the number of required resources
            if (score >= 0)
                score = (double)(score / (double)(iResCount));
            return score;
        }
    }

}
