Feature: Sample
  A sample feature showing how to run the file from xunit

  Background:
    Given Foo
"""
<Application />
"""

  Scenario: Foo should equal Bar
    Given Bar 
"""
<Application />
"""
    When running test
    And doing something
    Then foo equals bar
    And succeeds